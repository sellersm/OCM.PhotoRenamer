Option Explicit On
Option Strict On

Imports System.IO
Imports System.Configuration
Imports System.Xml.Serialization
Imports System.Xml
Imports Ionic.Utils.Zip
Imports Microsoft.Practices.EnterpriseLibrary.Logging
Imports FileHelpers

Public Class CRMPhotoRenamer
    <System.Runtime.InteropServices.DllImport("gdi32.dll")> _
    Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
    End Function

    Private _ProcessStatus As ProcessStatusType = ProcessStatusType.Stopped

    Private Property ProcessStatus() As ProcessStatusType
        Get
            Return _ProcessStatus
        End Get
        Set(ByVal value As ProcessStatusType)
            _ProcessStatus = value
        End Set
    End Property

    Public Enum ProcessStatusType
        Stopped
        Running
        Paused
    End Enum

    Private _TargetWidth As Integer = 0
    Private _TargetHeight As Integer = 0
    Private _PaddingTop As Decimal = 0
    Private _PaddingSide As Decimal = 0


    Private Sub HeadshotCropper_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = False
        If Me._ProcessStatus <> ProcessStatusType.Stopped Then
            If MsgBox("Headshots are currently being processed.  Are you sure you want to Exit?", MsgBoxStyle.YesNo, Me.Text) <> MsgBoxResult.Yes Then
                e.Cancel = True
            Else
                Me._ProcessStatus = ProcessStatusType.Stopped
            End If
        End If
    End Sub


    Private Sub HeadshotCropper_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.lblChildID.Text = ""
        Me.lblStatus.Text = ""
        Me.txtImportFileFolderName.SelectedText = ConfigurationManager.AppSettings("PhotoPath")

        SetButtons()


    End Sub


    Private Sub OpenFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim dlg As FolderBrowserDialog

        dlg = New FolderBrowserDialog()
        dlg.SelectedPath = txtSourceFileName.Text
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtSourceFileName.Text = dlg.SelectedPath
        End If
    End Sub

    Private Sub cmdProcessFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdProcessFolder.Click

        Dim PhotosProcessed As Dictionary(Of String, Boolean) = Nothing
        Dim ChildrenProcessed As Dictionary(Of String, Boolean) = Nothing

        Dim CSVName As String = ""
        Dim PhotoZipName As String = ""
        Dim FixedPhotoZipName As String = ""

        Dim NumberOfPhotosProcessed As Integer = 0
        Dim NumberOfPhotosChecked As Integer = 0
        Dim NumberOfInvalid As Integer = 0

        Dim PhotoZip As ZipFile = Nothing

        System.Threading.Thread.Sleep(1000)
        PhotosProcessed = New Dictionary(Of String, Boolean)
        ChildrenProcessed = New Dictionary(Of String, Boolean)

        Dim FullPath As String
        FullPath = txtSourceFileName.Text

        'Get Import File Folder
        Dim ImportFileFolder As String
        ImportFileFolder = txtImportFileFolderName.Text
        Dim FullPathName As String = ""

        'Get full path from UI
        FullPathName = Path.GetFileNameWithoutExtension(FullPath)
        CSVName = Path.GetFileName(FullPath)

        Dim engine As New FileHelperEngine(Of ChildIDMap)()
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue

        Dim ChildIDList As ChildIDMap() = engine.ReadFile(FullPath)

        If engine.ErrorManager.ErrorCount > 0 Then
            engine.ErrorManager.SaveErrors("Errors.txt")
        End If

        Dim FixDuplicateTempChildID As Boolean = False
        If cbDupTempChildID.Checked Then FixDuplicateTempChildID = True

        Dim ErrorList As List(Of String) = New List(Of String)()
        Dim ProcessedZipFileList As List(Of String) = New List(Of String)()

        For Each oneChildIDMap As ChildIDMap In ChildIDList

            NumberOfPhotosChecked += 1
            Dim OldPhotoName As String = ""
            Dim FixedTempChildID As String = ""
            Dim FixedChildID As String = ""

            FixedTempChildID = oneChildIDMap.TempChildID.Replace(Char.Parse(""""), String.Empty)

            Try
                If PhotoZipName <> oneChildIDMap.ZipFileName Then
                    If PhotoZip IsNot Nothing Then
                        PhotoZip.Dispose()
                    End If

                    'Need to remove additional quotation marks from oneChildIDMap objects
                    PhotoZipName = String.Format("{0} - Photos.zip", oneChildIDMap.ZipFileName.Trim()).Replace(Char.Parse(""""), String.Empty)

                    ' Get Zip File
                    PhotoZip = ZipFile.Read(Path.Combine(ImportFileFolder, PhotoZipName))
                    PhotoZip.CaseSensitiveRetrieval = False

                End If

                'If the FixDuplicateTempChildID checkbox is turned on we need to remove a character off the end of the TempChildID
                If FixDuplicateTempChildID Then
                    PhotoZip.Extract(FixedTempChildID.Substring(0, FixedTempChildID.Length - 1) & ".jpg", ImportFileFolder, True)
                    OldPhotoName = String.Format("{0}.jpg", Path.Combine(ImportFileFolder, FixedTempChildID.Substring(0, FixedTempChildID.Length - 1)))
                Else
                    PhotoZip.Extract(FixedTempChildID & ".jpg", ImportFileFolder, True)
                    OldPhotoName = String.Format("{0}.jpg", Path.Combine(ImportFileFolder, FixedTempChildID))
                End If

                'Compare the image to the "No Image" .jpg and process accordingly
                If CompareFile(OldPhotoName, ConfigurationManager.AppSettings("NoPhotoJPG")) Then
                    'If this photo is the same file (can have different file names) as the one used in HQ when the photo is unusable,
                    'then move the photo to the error folder and note that it is a "No Photo" jpg, so that it doesn't get imported into FileNexus
                    Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathError"), "NO PHOTO - " & oneChildIDMap.TempChildID & ".jpg")

                    'Move the photo to the Error folder
                    Try
                        File.Move(OldPhotoName, NewPhotoName)
                    Catch ex As Exception
                        ErrorList.Add(String.Format("Photo move failed for record {0} - {1}", FixedTempChildID, ex.Message))
                    End Try

                Else
                    'If it isn't a "No Photo" jpg, then rename the photo with the RE Child ID (constituent id) and move to the PhotoPathRenamed folder
                    'Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathRenamed"), oneChildIDMap.ChildID & ".jpg")
                    FixedChildID = oneChildIDMap.ChildID.Replace(Char.Parse(""""), String.Empty)
                    Dim NewPhotoName As String = String.Format("{0}.jpg", Path.Combine(ConfigurationManager.AppSettings("PhotoPathRenamed"), FixedChildID))

                    'Move the photo to the Successfully Renamed Folder
                    Try
                        File.Move(OldPhotoName, NewPhotoName)
                    Catch ex As Exception
                        ErrorList.Add(String.Format("Moving the Photo failed for record {0} - {1}", FixedTempChildID, ex.Message))
                    End Try

                End If

                ProcessedZipFileList.Add(PhotoZipName.ToString())
                NumberOfPhotosProcessed += 1

            Catch ex As Exception
                NumberOfInvalid += 1
                ErrorList.Add(String.Format("Photo rename failed for record {0} - {1}", FixedTempChildID, ex.Message))
            End Try

        Next

        If PhotoZip IsNot Nothing Then
            PhotoZip.Dispose()
        End If

        'If no photos were able to be renamed, then show a custom error first suggesting selecting the 
        'Duplicate ID checkbox
        If NumberOfPhotosProcessed = 0 Then
            rtbMessage.Text = "No Photos were renamed.  If this file was created as duplicate, please check the Duplicate Child ID check box"
        Else
            'Display results of the renaming
            rtbMessage.Text = "Photo rename complete. " & NumberOfPhotosProcessed.ToString & " photos were renamed."

            'Move processed file
            Dim NewFileFolderName As String = String.Format("{0}.csv", Path.Combine(ConfigurationManager.AppSettings("CSVPathProcessed"), FullPathName))
            Try
                File.Move(FullPath, NewFileFolderName)
            Catch ex As Exception
                ErrorList.Add("Moving the Renamed Photo failed - " & ex.Message)
            End Try

            'Move Processed Zip Files to the Processed Zip Folder
            For Each ProcessedZipFile As String In ProcessedZipFileList.Distinct()
                Try
                    Dim ProcessedZipFilePath = String.Format("{0}", Path.Combine(ImportFileFolder, ProcessedZipFile))
                    Dim NewZipFileFolder As String = String.Format("{0}", Path.Combine(ConfigurationManager.AppSettings("PhotoPathProcessedZip"), ProcessedZipFile))
                    File.Copy(ProcessedZipFilePath, NewZipFileFolder)

                    'Custom error giving directions for a deletion error
                    Try
                        File.Delete(ProcessedZipFilePath)
                    Catch ex As Exception
                        ErrorList.Add(String.Format("The file: {0} - Photos.zip was successfully moved to {1} but could not be deleted from {2}. It may be necessary to manually delete it.", ProcessedZipFile, ConfigurationManager.AppSettings("PhotoPathProcessedZip"), FullPath))
                    End Try

                Catch ex As Exception
                    ErrorList.Add(String.Format("Moving the Photo Zip File failed for record {0} - {1}", ProcessedZipFile, ex.Message))
                End Try
            Next

            'Display all errors, if there are any
            If ErrorList.Count > 0 Then
                For Each Item In ErrorList
                    rtbMessage.AppendText(Environment.NewLine & Item.ToString())
                Next
            End If

        End If

        lblNumProcessedValue.Text = NumberOfPhotosChecked.ToString()
        lblNumInvalid.Text = NumberOfInvalid.ToString()

    End Sub


    Private Sub SetButtons()
        If Me.ProcessStatus = ProcessStatusType.Running Then
            cmdProcessFolder.Enabled = False
            cmdCancel.Visible = True
            cmdPause.Visible = True
            cmdCopyList.Visible = False
        Else
            cmdProcessFolder.Enabled = True
            cmdCancel.Visible = False
            cmdPause.Visible = False
            If rtbMessage.TextLength > 0 Then
                cmdCopyList.Visible = True
            Else
                cmdCopyList.Visible = False
            End If
        End If
    End Sub

    Private Sub cmdBrowseFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseFolder.Click
        'Dim dlg As FolderBrowserDialog
        'dlg = New FolderBrowserDialog()
        'dlg.SelectedPath = txtSourceFolderName.Text

        Dim dlg As OpenFileDialog

        dlg = New OpenFileDialog()
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtSourceFileName.Text = dlg.FileName
            dlg.Filter = "CSV FILES|*.csv"
        End If

        txtSourceFileName.Focus()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        If Me.ProcessStatus = ProcessStatusType.Stopped Then
            Application.Exit()
        Else
            If MsgBox("Are you sure you want to stop creating import file?", MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
                Me.ProcessStatus = ProcessStatusType.Stopped
            End If
        End If
    End Sub

    Private Sub cmdPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPause.Click
        If Me.ProcessStatus = ProcessStatusType.Running Then
            Me.ProcessStatus = ProcessStatusType.Paused
            cmdPause.Text = "Resume"
            lblStatus.Text = "Paused..."
        Else
            Me.ProcessStatus = ProcessStatusType.Running
            cmdPause.Text = "Pause"
            lblStatus.Text = "Adding to import file...."
        End If
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


    End Sub

    Private Sub cmdCopyList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyList.Click
        rtbMessage.SelectAll()
        rtbMessage.Copy()
        MsgBox("The results list has been copied to clipboard.")
    End Sub


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim dlg As FolderBrowserDialog

        dlg = New FolderBrowserDialog()
        dlg.SelectedPath = txtImportFileFolderName.Text
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtImportFileFolderName.Text = dlg.SelectedPath
        End If

        txtSourceFileName.Focus()
    End Sub

    Public Function CompareFile(ByVal fileA As String, ByVal fileB As String) As Boolean

        If fileA Is Nothing Then
            Throw New NullReferenceException("fileA cannot be null")
        End If

        If Not File.Exists(fileA) Then
            Throw New NullReferenceException("'" & fileA & "' does not exist")
        End If

        If fileB Is Nothing Then
            Throw New NullReferenceException("fileB cannot be null")
        End If

        If Not File.Exists(fileB) Then
            Throw New NullReferenceException("'" & fileB & "' does not exist")
        End If

        Dim Hash As System.Security.Cryptography.HashAlgorithm
        Hash = System.Security.Cryptography.HashAlgorithm.Create()

        'Create hash for first file
        Dim FSA As New FileStream(fileA, FileMode.Open)
        Dim HashA() As Byte = Hash.ComputeHash(FSA)
        FSA.Close()

        'Create hash for second file
        Dim FSB As New FileStream(fileB, FileMode.Open)
        Dim HashB() As Byte = Hash.ComputeHash(FSB)
        FSB.Close()

        'Return the comparison of the two hash codes
        Return BitConverter.ToString(HashA) = BitConverter.ToString(HashB)

    End Function

End Class
<IgnoreFirst(1), _
 DelimitedRecord(",")> _
Public NotInheritable Class ChildIDMap


    Private mBUSINESSPROCESSOUTPUT_PKID As String

    Public Property BUSINESSPROCESSOUTPUT_PKID As String
        Get
            Return mBUSINESSPROCESSOUTPUT_PKID
        End Get
        Set(value As String)
            mBUSINESSPROCESSOUTPUT_PKID = value
        End Set
    End Property


    Private mTempChildID As String

    Public Property TempChildID As String
        Get
            Return mTempChildID
        End Get
        Set(value As String)
            mTempChildID = value
        End Set
    End Property


    Private mChildID As String

    Public Property ChildID As String
        Get
            Return mChildID
        End Get
        Set(value As String)
            mChildID = value
        End Set
    End Property


    Private mZipFileName As String

    Public Property ZipFileName As String
        Get
            Return mZipFileName
        End Get
        Set(value As String)
            mZipFileName = value
        End Set
    End Property



End Class


