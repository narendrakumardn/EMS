<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"  CodeBehind="StudentUpload.aspx.cs" Inherits="BTEDiploma.admin.StudentUpload" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="True" />
    <div class="container mt-4">
        <h3>Upload Student Details</h3>

          <div style="padding:20px">
            <asp:Button ID="btnDownloadTemplate" runat="server" 
                Text="Download Student Upload Template"
                OnClick="btnDownloadTemplate_Click"
                CssClass="btn btn-primary" />
        </div>
    </div>
    
        <asp:FileUpload ID="fuExcel" runat="server" />
<asp:Button ID="btnUpload" runat="server" Text="Upload Excel" OnClick="btnUpload_Click" />

<asp:GridView ID="gvPreview" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Student Name" />
        
        <asp:BoundField DataField="Program_Code" HeaderText="Program" />
        <asp:BoundField DataField="Institution_Code" HeaderText="College" />
        <asp:BoundField DataField="AdharNumber" HeaderText="AdharNumber " />
        <asp:BoundField DataField="Application_Number" HeaderText="Application_Number" />
        <asp:BoundField DataField="SATS_Number" HeaderText="SATS_Number " />
        <asp:BoundField DataField="Father_Name" HeaderText="Father Name" />
        <asp:BoundField DataField="Mother_Name" HeaderText="Mother Name" />
        <asp:BoundField DataField="Address" HeaderText="Address" />
         
 <asp:BoundField DataField="PINcode" HeaderText="PINcode" />
 <asp:BoundField DataField="TalukID" HeaderText="TalukID" />
 <asp:BoundField DataField="Cat_Code" HeaderText="Category Code" />
 <asp:BoundField DataField="Gender" HeaderText="Gender" />
 <asp:BoundField DataField="Admission_Type" HeaderText="Admission_Type" />
 <asp:BoundField DataField="Date_Of_Birth" HeaderText="Date_Of_Birth " />
 <asp:BoundField DataField="Email_ID" HeaderText="Email_ID" />
 <asp:BoundField DataField="SNQ" HeaderText="SNQ" />
    </Columns>
</asp:GridView>
    <asp:GridView ID="gvInserted" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered" />

<asp:Button ID="btnSubmit" runat="server" Text="Final Submit" OnClick="btnSubmit_Click" />


    
</asp:Content>