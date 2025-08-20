<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddExamFeeDetails.aspx.cs" Inherits="BTEDiploma.admin.AddExamFeeDetailscollege" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <%-- If you use AjaxControlToolkit calendar, register it on master or here:
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %> --%>
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-white bg-primary p-3 rounded">Add Exam Fee Details</h2>

        <asp:UpdatePanel ID="updMain" runat="server">
            <ContentTemplate>
                <div class="row g-3 mt-3">
                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlAcademicYear" Text="Academic Year" CssClass="form-label" />
                        <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged" />
                    </div>

                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlExamYear" Text="Semester Odd/Even" CssClass="form-label" />
                        <asp:DropDownList ID="ddlExamYear" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlSemester_SelectedIndexChanged" />
                    </div>

                    

                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlProgramme" Text="Programme" CssClass="form-label" />
                        <asp:DropDownList ID="ddlProgramme" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" />
                    </div>

                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlSemester" Text="Semester" CssClass="form-label" />
                        <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged">
                            <asp:ListItem Text="--Select--" Value="" />
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="6" Value="6" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-12">
                        <asp:Label ID="lblMessage" runat="server" CssClass="fw-semibold"></asp:Label>
                    </div>
                </div>

                <hr class="my-4" />

                <%-- Hidden page-level payment mode list for your existing code-behind LoadPaymentModes() call --%>
                <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-control" Style="display:none;" />
                <div class="alert alert-warning fw-semibold">
    ⚠ Please verify student details before saving or updating payment records.
</div>
                <asp:Label ID="lblCaution" runat="server" ForeColor="Red" EnableViewState="false" />
<br />
              <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False"
    CssClass="table table-bordered table-striped"
    DataKeyNames="Sem_History_ID,Exam_ID,Payment_ID"
    OnRowDataBound="gvStudents_RowDataBound"
    OnRowCommand="gvStudents_RowCommand">

    <Columns>
        <asp:TemplateField HeaderText="S.No">
    <ItemTemplate>
        <%# Container.DataItemIndex + 1 %>
    </ItemTemplate>
    <ItemStyle CssClass="text-center px-3 py-2" />
    <HeaderStyle CssClass="bg-light fw-bold text-center px-3 py-2" />
</asp:TemplateField>
        <asp:BoundField DataField="Register_Number" HeaderText="Reg No" />
        <asp:BoundField DataField="Student_Name" HeaderText="Name" />

      <%-- Payment Date --%>
<asp:TemplateField HeaderText="Payment Date">
    <ItemTemplate>
        <asp:TextBox ID="txtPayDate" runat="server" 
            Text='<%# Eval("Payment_Date", "{0:yyyy-MM-dd}") %>' 
            CssClass="form-control" />
    </ItemTemplate>
</asp:TemplateField>


        

        <%-- Amount --%>
<asp:TemplateField HeaderText="Amount">
    <ItemTemplate>
        <asp:TextBox ID="txtAmount" runat="server" 
            Text='<%# Eval("Fee_Amount") %>' CssClass="form-control" />
    </ItemTemplate>
</asp:TemplateField>

       
        <%-- Mode --%>
        <asp:TemplateField HeaderText="Mode">
            <ItemTemplate>
                <asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control"></asp:DropDownList>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Ref No">
            <ItemTemplate>
                <asp:TextBox ID="txtRefNo" runat="server" Text='<%# Eval("Transaction_Ref_No") %>' CssClass="form-control" />
            </ItemTemplate>
        </asp:TemplateField>

       
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:HiddenField ID="hfSemHistoryID" runat="server" Value='<%# Eval("Sem_History_ID") %>' />
                <asp:HiddenField ID="hfExamID" runat="server" Value='<%# Eval("Exam_ID") %>' />
                <asp:HiddenField ID="hfPaymentID" runat="server" Value='<%# Eval("Payment_ID") %>' />

                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success btn-sm me-1"
                    CommandName="SaveRow" CommandArgument="<%# Container.DataItemIndex %>" />

                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-secondary btn-sm"
                    CommandName="EditRow" CommandArgument="<%# Container.DataItemIndex %>" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

                <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger mt-3" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

