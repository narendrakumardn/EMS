<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateProgram.aspx.cs" Inherits="BTEDiploma.admin.CreateProgram" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
     <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
     <script src="https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js"></script>
 <style type="text/css">
     .pagination-outer a, .pagination-outer span {
         margin: 2px;
         padding: 6px 12px;
         border: 1px solid #dee2e6;
         border-radius: 5px;
         text-decoration: none;
     }
     .pagination-outer span {
         background-color: #0d6efd;
         color: white;
         font-weight: bold;
     }
 </style>
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white">
                <h4 class="mb-0">Create Program</h4>
            </div>
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label">Program Code</label>
                        <asp:TextBox ID="txtProgramCode" runat="server" CssClass="form-control" MaxLength="4" required="required" />
                    </div>
                    <div class="col-md-8">
                        <label class="form-label">Program Name</label>
                        <asp:TextBox ID="txtProgramName" runat="server" CssClass="form-control" MaxLength="100" required="required" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Program Type</label>
                        <asp:DropDownList ID="ddlProgramType" runat="server" CssClass="form-select" required="required">
                            <asp:ListItem Text="-- Select Type --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Min Credit to Award Diploma</label>
                        <asp:TextBox ID="txtminCredit" runat="server" CssClass="form-control" TextMode="Number" min="0" required="required" />
                    </div>
                    <div class="col-md-12">
                        <label class="form-label">Diploma Award Title</label>
                        <asp:TextBox ID="txtDiplomaAwardTitle" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    <div class="col-12 mt-3">
                        <asp:Button ID="btnAddProgram" runat="server" Text="Add Program" CssClass="btn btn-success me-2" OnClick="btnAddProgram_Click" />
                        <asp:Button ID="btnClearProgram" runat="server" Text="Clear Fields" CssClass="btn btn-secondary" OnClick="ClearFields" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    
        <div class="card mt-4 shadow-sm">
            <div class="card-header bg-secondary text-white">
                <h5 class="mb-0">Curriculum Scheme Records</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvProgram" runat="server" AutoGenerateColumns="False" 

                AllowPaging="true"
                PageSize="10"
                OnPageIndexChanging="gvProgram_PageIndexChanging"
                DataKeyNames="Program_Code"
                OnRowEditing="gvProgram_RowEditing"
                OnRowUpdating="gvProgram_RowUpdating"
                OnRowCancelingEdit="gvProgram_RowCancelingEdit"
                OnRowDataBound="gvProgram_RowDataBound"
                CssClass="table table-bordered table-striped table-hover">
                    <Columns>
                        <asp:TemplateField HeaderText="Program Code">
                        <ItemTemplate>
                            <asp:Label ID="lblProgramCode" runat="server" Text='<%# Eval("Program_Code") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                                
                        <asp:TemplateField HeaderText="Program Name">
    <ItemTemplate>
        <asp:Label ID="lblProgramName" runat="server" Text='<%# Eval("Program_Name") %>'></asp:Label>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtProgramName" runat="server" Text='<%# Bind("Program_Name") %>'></asp:TextBox>
    </EditItemTemplate>
</asp:TemplateField>
                    <asp:TemplateField HeaderText="Program Type">
    <EditItemTemplate>
        <asp:DropDownList ID="ddlProgramTypeGrid" runat="server" CssClass="form-select">
        </asp:DropDownList>
    </EditItemTemplate>
    <ItemTemplate>
        <%# Eval("Program_Type_Name") %> 
    </ItemTemplate>
</asp:TemplateField>


<asp:TemplateField HeaderText="Min Credits Required">
    <ItemTemplate>
        <asp:Label ID="lblCredit" runat="server" Text='<%# Eval("Credit") %>'></asp:Label>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtCredit" runat="server" Text='<%# Bind("Credit") %>'></asp:TextBox>
    </EditItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Diploma Title">
    <ItemTemplate>
        <asp:Label ID="lblDiplomaTitle" runat="server" Text='<%# Eval("Diploma_Title") %>'></asp:Label>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtDiplomaTitle" runat="server" Text='<%# Bind("Diploma_Title") %>'></asp:TextBox>
    </EditItemTemplate>
</asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" 
                                    CommandName="Edit" 
                                    CssClass="btn btn-sm btn-outline-primary me-1"
                                    Text="Edit"></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkUpdate" runat="server" 
                                    CommandName="Update" 
                                    CssClass="btn btn-sm btn-success me-1"
                                    Text="Update"></asp:LinkButton>
                                <asp:LinkButton ID="lnkCancel" runat="server" 
                                    CommandName="Cancel" 
                                    CssClass="btn btn-sm btn-secondary"
                                    Text="Cancel"></asp:LinkButton>
                            </EditItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <PagerStyle CssClass="pagination-outer" HorizontalAlign="Center" />
                </asp:GridView></div></div>
</asp:Content>
