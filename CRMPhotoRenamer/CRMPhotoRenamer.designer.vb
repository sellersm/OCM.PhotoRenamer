﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CRMPhotoRenamer
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.lblNumProcessed = New System.Windows.Forms.Label()
        Me.lblNumProcessedValue = New System.Windows.Forms.Label()
        Me.txtSourceFileName = New System.Windows.Forms.TextBox()
        Me.cmdProcessFolder = New System.Windows.Forms.Button()
        Me.rtbMessage = New System.Windows.Forms.RichTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblChildID = New System.Windows.Forms.Label()
        Me.cmdBrowseFolder = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdPause = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblNumInvalid = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmdCopyList = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtImportFileFolderName = New System.Windows.Forms.TextBox()
        Me.cbDupTempChildID = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblNumProcessed
        '
        Me.lblNumProcessed.AutoSize = True
        Me.lblNumProcessed.Location = New System.Drawing.Point(10, 36)
        Me.lblNumProcessed.Name = "lblNumProcessed"
        Me.lblNumProcessed.Size = New System.Drawing.Size(93, 13)
        Me.lblNumProcessed.TabIndex = 2
        Me.lblNumProcessed.Text = "Number Checked:"
        '
        'lblNumProcessedValue
        '
        Me.lblNumProcessedValue.AutoSize = True
        Me.lblNumProcessedValue.Location = New System.Drawing.Point(109, 36)
        Me.lblNumProcessedValue.Name = "lblNumProcessedValue"
        Me.lblNumProcessedValue.Size = New System.Drawing.Size(13, 13)
        Me.lblNumProcessedValue.TabIndex = 3
        Me.lblNumProcessedValue.Text = "0"
        '
        'txtSourceFileName
        '
        Me.txtSourceFileName.Location = New System.Drawing.Point(12, 389)
        Me.txtSourceFileName.Name = "txtSourceFileName"
        Me.txtSourceFileName.Size = New System.Drawing.Size(503, 20)
        Me.txtSourceFileName.TabIndex = 4
        '
        'cmdProcessFolder
        '
        Me.cmdProcessFolder.Location = New System.Drawing.Point(9, 454)
        Me.cmdProcessFolder.Name = "cmdProcessFolder"
        Me.cmdProcessFolder.Size = New System.Drawing.Size(130, 36)
        Me.cmdProcessFolder.TabIndex = 5
        Me.cmdProcessFolder.Text = "Process Folder"
        Me.cmdProcessFolder.UseVisualStyleBackColor = True
        '
        'rtbMessage
        '
        Me.rtbMessage.BackColor = System.Drawing.SystemColors.Control
        Me.rtbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbMessage.ForeColor = System.Drawing.Color.Navy
        Me.rtbMessage.Location = New System.Drawing.Point(11, 94)
        Me.rtbMessage.Name = "rtbMessage"
        Me.rtbMessage.ReadOnly = True
        Me.rtbMessage.Size = New System.Drawing.Size(531, 275)
        Me.rtbMessage.TabIndex = 6
        Me.rtbMessage.Text = ""
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 372)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Source File:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 14)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Last Photo:"
        '
        'lblChildID
        '
        Me.lblChildID.AutoSize = True
        Me.lblChildID.Location = New System.Drawing.Point(109, 14)
        Me.lblChildID.Name = "lblChildID"
        Me.lblChildID.Size = New System.Drawing.Size(41, 13)
        Me.lblChildID.TabIndex = 9
        Me.lblChildID.Text = "ChildID"
        '
        'cmdBrowseFolder
        '
        Me.cmdBrowseFolder.Location = New System.Drawing.Point(521, 389)
        Me.cmdBrowseFolder.Name = "cmdBrowseFolder"
        Me.cmdBrowseFolder.Size = New System.Drawing.Size(24, 22)
        Me.cmdBrowseFolder.TabIndex = 10
        Me.cmdBrowseFolder.Text = "..."
        Me.cmdBrowseFolder.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(467, 467)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 11
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdPause
        '
        Me.cmdPause.Location = New System.Drawing.Point(386, 467)
        Me.cmdPause.Name = "cmdPause"
        Me.cmdPause.Size = New System.Drawing.Size(75, 23)
        Me.cmdPause.TabIndex = 12
        Me.cmdPause.Text = "Pause"
        Me.cmdPause.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.ForeColor = System.Drawing.Color.Maroon
        Me.lblStatus.Location = New System.Drawing.Point(287, 34)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(90, 15)
        Me.lblStatus.TabIndex = 13
        Me.lblStatus.Text = "Not Processing"
        '
        'lblNumInvalid
        '
        Me.lblNumInvalid.AutoSize = True
        Me.lblNumInvalid.Location = New System.Drawing.Point(251, 36)
        Me.lblNumInvalid.Name = "lblNumInvalid"
        Me.lblNumInvalid.Size = New System.Drawing.Size(13, 13)
        Me.lblNumInvalid.TabIndex = 15
        Me.lblNumInvalid.Text = "0"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(152, 36)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(81, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Number Invalid:"
        '
        'cmdCopyList
        '
        Me.cmdCopyList.Location = New System.Drawing.Point(166, 454)
        Me.cmdCopyList.Name = "cmdCopyList"
        Me.cmdCopyList.Size = New System.Drawing.Size(129, 36)
        Me.cmdCopyList.TabIndex = 16
        Me.cmdCopyList.Text = "Copy List"
        Me.cmdCopyList.UseVisualStyleBackColor = True
        Me.cmdCopyList.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(522, 428)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 22)
        Me.Button1.TabIndex = 19
        Me.Button1.Text = "..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 411)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 13)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "Import File Folder:"
        '
        'txtImportFileFolderName
        '
        Me.txtImportFileFolderName.Location = New System.Drawing.Point(13, 428)
        Me.txtImportFileFolderName.Name = "txtImportFileFolderName"
        Me.txtImportFileFolderName.Size = New System.Drawing.Size(503, 20)
        Me.txtImportFileFolderName.TabIndex = 17
        '
        'cbDupTempChildID
        '
        Me.cbDupTempChildID.AutoSize = True
        Me.cbDupTempChildID.Location = New System.Drawing.Point(13, 61)
        Me.cbDupTempChildID.Name = "cbDupTempChildID"
        Me.cbDupTempChildID.Size = New System.Drawing.Size(141, 17)
        Me.cbDupTempChildID.TabIndex = 20
        Me.cbDupTempChildID.Text = "Duplicate Temp Child ID"
        Me.cbDupTempChildID.UseVisualStyleBackColor = True
        '
        'CRMPhotoRenamer
        '
        Me.AcceptButton = Me.cmdProcessFolder
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(554, 504)
        Me.Controls.Add(Me.cbDupTempChildID)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtImportFileFolderName)
        Me.Controls.Add(Me.cmdCopyList)
        Me.Controls.Add(Me.lblNumInvalid)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.cmdPause)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdBrowseFolder)
        Me.Controls.Add(Me.lblChildID)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.rtbMessage)
        Me.Controls.Add(Me.cmdProcessFolder)
        Me.Controls.Add(Me.txtSourceFileName)
        Me.Controls.Add(Me.lblNumProcessedValue)
        Me.Controls.Add(Me.lblNumProcessed)
        Me.Name = "CRMPhotoRenamer"
        Me.Text = "CRM Photo Renamer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblNumProcessed As System.Windows.Forms.Label
    Friend WithEvents lblNumProcessedValue As System.Windows.Forms.Label
    Friend WithEvents txtSourceFileName As System.Windows.Forms.TextBox
    Friend WithEvents cmdProcessFolder As System.Windows.Forms.Button
    Friend WithEvents rtbMessage As System.Windows.Forms.RichTextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblChildID As System.Windows.Forms.Label
    Friend WithEvents cmdBrowseFolder As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdPause As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblNumInvalid As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cmdCopyList As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtImportFileFolderName As System.Windows.Forms.TextBox
    Friend WithEvents cbDupTempChildID As System.Windows.Forms.CheckBox

End Class
