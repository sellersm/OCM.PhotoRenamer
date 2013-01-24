Option Explicit On
Option Strict On

Imports System.io
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

		SetButtons()


	End Sub


	Private Sub OpenFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If
	End Sub

	Private Sub cmdProcessFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdProcessFolder.Click
		Dim OneLogEntry As LogEntry = Nothing
		Dim PhotosProcessed As Dictionary(Of String, Boolean) = Nothing
		Dim ChildrenProcessed As Dictionary(Of String, Boolean) = Nothing

		Dim CSVName As String = ""
		Dim PhotoZipName As String = ""


		Dim NumberOfPhotosProcessed As Integer = 0

		Dim PhotoZip As ZipFile = Nothing

		Try
			System.Threading.Thread.Sleep(1000)
			PhotosProcessed = New Dictionary(Of String, Boolean)
			ChildrenProcessed = New Dictionary(Of String, Boolean)

			'*** Joe - Get full path from UI

			'PhotoZipName = Path.GetFileNameWithoutExtension(e.FullPath) & " - Photos.zip"
			CSVName = Path.GetFileName("C:\MoM\Loaded\Photo Renamer 12-31-12.csv")

			' Get Child List from CSV file
			'Using fs As New FileStream(e.FullPath, FileMode.Open)

			'	Dim deserializer As New XmlSerializer(GetType(ChildIDList))
			'	Dim reader As New XmlTextReader(fs)

			'	ChildList = CType(deserializer.Deserialize(reader), ChildIDList)
			'	fs.Close()
			'End Using

			Dim engine As New FileHelperEngine(Of ChildIDMap)()

			engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue

			'*** Joe - Get full path from UI (same as above CSV Name)

			Dim ChildIDList As ChildIDMap() = engine.ReadFile("C:\MoM\Loaded\Photo Renamer 12-31-12.csv")

			If engine.ErrorManager.ErrorCount > 0 Then
				engine.ErrorManager.SaveErrors("Errors.txt")
			End If

			For Each oneChildIDMap As ChildIDMap In ChildIDList
				If PhotoZipName <> oneChildIDMap.ZipFileName Then
					If PhotoZip IsNot Nothing Then
						PhotoZip.Dispose()
					End If
					PhotoZipName = oneChildIDMap.ZipFileName & " - Photos.zip"

					'*** Joe - Fix Log issue

					'OneLogEntry = New LogEntry()
					'OneLogEntry.EventId = 101
					'OneLogEntry.Priority = 2
					'OneLogEntry.Message = "Processing XML Loader file - " & CSVName & ", Photo Zip File - " & PhotoZipName
					'OneLogEntry.Categories.Add("General")
					'Logger.Write(OneLogEntry)

					' Get Zip File
					PhotoZip = ZipFile.Read(Path.Combine(ConfigurationManager.AppSettings("PhotoPath"), PhotoZipName))
					PhotoZip.CaseSensitiveRetrieval = False

				End If

				Dim FixDuplicateTempChildID As Boolean = False
				Dim OldPhotoName As String = ""

				If FixDuplicateTempChildID Then
					PhotoZip.Extract(oneChildIDMap.TempChildID.Substring(0, oneChildIDMap.TempChildID.Length - 1) & ".jpg", ConfigurationManager.AppSettings("PhotoPath"))
					OldPhotoName = Path.Combine(ConfigurationManager.AppSettings("PhotoPath"), oneChildIDMap.TempChildID.Substring(0, oneChildIDMap.TempChildID.Length - 1) & ".jpg")
				Else
					PhotoZip.Extract(oneChildIDMap.TempChildID & ".jpg", ConfigurationManager.AppSettings("PhotoPath"))
					OldPhotoName = Path.Combine(ConfigurationManager.AppSettings("PhotoPath"), oneChildIDMap.TempChildID & ".jpg")
				End If

				'*** Joe - Re add this logic. Get No Photo jpg from Cary.
				'If CompareFile(OldPhotoName, ConfigurationManager.AppSettings("NoPhotoJPG")) Then
				'	'If CompareFile(OldPhotoName, "C:\MoM\ChildPhotos\No Photo\No Photo.JPG") Then
				'	'If this photo is the same file (can have different file names) as the one used in HQ when the photo is unusable,
				'	'then move the photo to the error folder and note that it is a "No Photo" jpg, so that it doesn't get imported into FileNexus
				'	Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathError"), "NO PHOTO - " & oneChildIDMap.TempChildID & ".jpg")
				'	File.Move(OldPhotoName, NewPhotoName)
				'Else
				'If it isn't a "No Photo" jpg, then rename the photo with the RE Child ID (constituent id) and move to the PhotoPathRenamed folder
				Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathRenamed"), oneChildIDMap.ChildID & ".jpg")
				File.Move(OldPhotoName, NewPhotoName)
				NumberOfPhotosProcessed += 1
				'End If

			Next


			'*** Joe - Not sure if we need this.

			'' Initialize ChildrenProcessed dictionary as not found.
			'For Each Child As Child In ChildList.Children
			'	ChildrenProcessed.Add((Child.TempChildID & ".jpg").ToUpper, False)
			'Next

			'' Determine if the photo for the child is in the zip file and visa versa and log in dictionaries
			'For Each Photo As ZipEntry In PhotoZip.Entries
			'	If ChildrenProcessed.ContainsKey(Photo.FileName.ToUpper) Then
			'		ChildrenProcessed.Item(Photo.FileName.ToUpper) = True
			'		PhotosProcessed.Add(Photo.FileName.ToUpper, True)
			'	Else
			'		PhotosProcessed.Add(Photo.FileName.ToUpper, False)
			'	End If
			'Next

			'' If there are any key value pairs that contains false, then log error
			'If ChildrenProcessed.ContainsValue(False) Then
			'	OneLogEntry = New LogEntry()
			'	OneLogEntry.EventId = 101
			'	OneLogEntry.Priority = 2
			'	OneLogEntry.Message = "Child List does not match the photo zip file.  Children not in Photo Zip File"
			'	For Each ChildProcessed As KeyValuePair(Of String, Boolean) In ChildrenProcessed
			'		If Not ChildProcessed.Value Then
			'			OneLogEntry.Message &= ControlChars.NewLine & ControlChars.Tab & ChildProcessed.Key
			'		End If
			'	Next ChildProcessed

			'	OneLogEntry.Categories.Add("Error")

			'	Logger.Write(OneLogEntry)
			'End If

			''Process each child with a photo in the zip file
			'For Each OneChild As Child In ChildList.Children
			'	If ChildrenProcessed.Item((OneChild.TempChildID & ".jpg").ToUpper) Then
			'		'TempChildID jpg is in zip file
			'		'Extract photo
			'		PhotoZip.Extract(OneChild.TempChildID & ".jpg", ConfigurationManager.AppSettings("PhotoPath"))

			'		Dim OldPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPath"), OneChild.TempChildID & ".jpg")

			'		If CompareFile(OldPhotoName, ConfigurationManager.AppSettings("NoPhotoJPG")) Then
			'			'If CompareFile(OldPhotoName, "C:\MoM\ChildPhotos\No Photo\No Photo.JPG") Then
			'			'If this photo is the same file (can have different file names) as the one used in HQ when the photo is unusable,
			'			'then move the photo to the error folder and note that it is a "No Photo" jpg, so that it doesn't get imported into FileNexus
			'			Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathError"), "NO PHOTO - " & OneChild.TempChildID & ".jpg")
			'			File.Move(OldPhotoName, NewPhotoName)
			'		Else
			'			'If it isn't a "No Photo" jpg, then rename the photo with the RE Child ID (constituent id) and move to the PhotoPathRenamed folder
			'			Dim NewPhotoName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathRenamed"), OneChild.ChildID & ".jpg")
			'			File.Move(OldPhotoName, NewPhotoName)
			'			NumberOfPhotosProcessed += 1
			'		End If
			'	End If
			'Next OneChild

			'' If the photo zip contained photos that were not in the xml file log errors
			'If PhotosProcessed.ContainsValue(False) Then
			'	OneLogEntry = New LogEntry()
			'	OneLogEntry.EventId = 101
			'	OneLogEntry.Priority = 1
			'	OneLogEntry.Message = "Child List does not match the photo zip file.  Photos that are not in the Child List"
			'	For Each PhotoProcessed As KeyValuePair(Of String, Boolean) In PhotosProcessed
			'		If Not PhotoProcessed.Value Then
			'			' If the photo does not exist in the xml file, write error to logger and move file to the Error folder
			'			OneLogEntry.Message &= ControlChars.NewLine & ControlChars.Tab & PhotoProcessed.Key
			'			PhotoZip.Extract(PhotoProcessed.Key, ConfigurationManager.AppSettings("PhotoPathError"))
			'		End If
			'	Next PhotoProcessed

			'	OneLogEntry.Categories.Add("Error")

			'	Logger.Write(OneLogEntry)
			'End If

			'PhotoZip.Dispose()

			''Move processed files
			'Dim OldPhotoZipName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPath"), PhotoZipName)
			'Dim NewPhotoZipName As String = Path.Combine(ConfigurationManager.AppSettings("PhotoPathProcessedZip"), PhotoZipName)
			'File.Move(OldPhotoZipName, NewPhotoZipName)

			'Dim OldXmlName As String = Path.Combine(ConfigurationManager.AppSettings("LoadPath"), CSVName)
			'Dim NewXmlName As String = Path.Combine(ConfigurationManager.AppSettings("LoadPathProcessed"), CSVName)
			'File.Move(OldXmlName, NewXmlName)

		Catch ex As Exception
			OneLogEntry = New LogEntry()
			OneLogEntry.EventId = 300
			OneLogEntry.Priority = 1
			OneLogEntry.Message = "Photo rename failed - " & ex.Message
			OneLogEntry.Categories.Add("Error")
			Logger.Write(OneLogEntry)
		End Try

		'*** Joe - Fix Log issue

		'OneLogEntry = New LogEntry()
		'OneLogEntry.EventId = 101
		'OneLogEntry.Priority = 2
		'OneLogEntry.Message = "Photo rename complete. " & NumberOfPhotosProcessed.ToString & " photos were renamed."
		'OneLogEntry.Categories.Add("General")
		'Logger.Write(OneLogEntry)

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
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If

		txtSourceFolderName.Focus()
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

		txtSourceFolderName.Focus()
	End Sub
End Class
<IgnoreFirst(1), _
 DelimitedRecord(",")> _
Public NotInheritable Class ChildIDMap


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