<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"
    CodeBehind="StudentUploadApproval.aspx.cs" Inherits="BTEDiploma.admin.StudentUploadApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="upMain" runat="server">
        <ContentTemplate>

            <div class="container mt-4">

              
                <h3 class="text-center text-white py-2 mb-3 rounded" 
                    style="background-color:#007bff;">
                    Approve Student Details - Post approval from ACM
                </h3>

               
                <div class="alert alert-warning text-center fw-bold" role="alert">
                    Candidates without approval from ACM should be deselected here
                </div>

          
                <div class="row mb-3">
                    <div class="col-md-4">
                        <label class="fw-bold">Academic Year:</label>
                        <asp:DropDownList ID="ddlAcademicYear" runat="server" Width="100%" 
                            CssClass="form-select" />
                    </div>
                    <div class="col-md-4">
                        <label class="fw-bold">Institution:</label>
                      <asp:DropDownList ID="ddlInstitution" runat="server" AutoPostBack="true"
    OnSelectedIndexChanged="ddlInstitution_SelectedIndexChanged" CssClass="form-select" />

                    </div>
                    <div class="col-md-4">
                        <label class="fw-bold">Program:</label>
                        <asp:DropDownList ID="ddlProgram" runat="server" Width="100%" 
                            CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
                    </div>
                </div>

               
                <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="Student_ID,Student_Enrollment_ID"
                    CssClass="table table-bordered table-striped table-hover text-nowrap">

                    <Columns>
                        <asp:BoundField DataField="Student_ID" HeaderText="Student ID" />
                        <asp:BoundField DataField="Student_Enrollment_ID" HeaderText="Enrollment ID" />
                        <asp:BoundField DataField="Student_Name" HeaderText="Student Name" />
                        <asp:BoundField DataField="Register_Number" HeaderText="Register Number" />
                        <asp:BoundField DataField="Institution_Code" HeaderText="Institution" />
                        <asp:BoundField DataField="Program_Code" HeaderText="Program" />

                        <asp:TemplateField HeaderText="Approve ACM">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkApproved" runat="server" 
                                    Checked='<%# Convert.ToBoolean(Eval("Is_Approved_By_ACM")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

           
                <div class="mt-3 text-center">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Approval" 
                        CssClass="btn btn-success px-4 fw-bold" OnClick="btnSubmit_Click" />
                </div>

                <div class="text-center mt-2">
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Green" Font-Bold="true" />
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
