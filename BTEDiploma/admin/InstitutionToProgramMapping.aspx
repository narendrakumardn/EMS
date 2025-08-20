<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"  CodeBehind="InstituteProgramMapping.aspx.cs" Inherits="BTEDiploma.admin.InstituteProgramMapping" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h3 class="mb-4"><i class="fas fa-link"></i> Institution to Program Mapping</h3>


        <asp:UpdatePanel ID="updInstitution" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <!-- Institution Type Dropdown -->
        <div class="form-group">
            <label for="ddlInstitutionType"><i class="fas fa-building"></i> Institution Type</label>
            <div class="input-group">
                <asp:DropDownList ID="ddlInstitutionType" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlInstitutionType_SelectedIndexChanged"
                    CssClass="form-control custom-dropdown" />
                <div class="input-group-append">
                </div>
            </div>
        </div>

        <!-- Institution Dropdown -->
        <div class="form-group">
            <label for="ddlInstitution"><i class="fas fa-school"></i> Select Institution</label>
            <div class="input-group">
                <asp:DropDownList ID="ddlInstitution" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlInstitution_SelectedIndexChanged"
                    CssClass="form-control custom-dropdown" />
                <div class="input-group-append">
                 
                </div>
            </div>
        </div>

        <!-- Add New Mapping Panel -->
        <asp:Panel ID="pnlAddNew" runat="server" Visible="false" CssClass="card p-4 shadow-sm mt-4">
            <h5><i class="fas fa-plus-circle text-success"></i> Add New Mapping</h5>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <label><i class="fas fa-graduation-cap"></i> Program</label>
                    <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-control custom-dropdown" />
                </div>

                <div class="form-group col-md-2">
                    <label><i class="fas fa-calendar"></i> Affiliation Year</label>
                    <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" placeholder="e.g. 2025" />
                </div>

                <div class="form-group col-md-2">
                    <label><i class="fas fa-user-friends"></i> Approved Intake</label>
                    <asp:DropDownList ID="ddlIntake" runat="server" CssClass="form-control">
                        <asp:ListItem Text="60" Value="60" />
                        <asp:ListItem Text="63" Value="63" />
                        <asp:ListItem Text="40" Value="40" />
                        <asp:ListItem Text="30" Value="30" />
                    </asp:DropDownList>
                </div>

                <div class="form-group col-md-2">
                    <label>&nbsp;</label>
                    <div class="form-check">
                        <asp:CheckBox ID="chkActive" runat="server" Text=" Active" CssClass="form-check-input" />
                        <label class="form-check-label" for="chkActive"></label>
                    </div>
                </div>

                <div class="form-group col-md-2">
                    <label>&nbsp;</label>
                    <div class="form-check">
                        <asp:CheckBox ID="chkAided" runat="server" Text=" Govt/Aided" CssClass="form-check-input" />
                        <label class="form-check-label" for="chkAided"></label>
                    </div>
                </div>

            
<!-- Button OUTSIDE form-row, aligned to right -->
<div class="d-flex justify-content-end mt-3">
    <asp:Button ID="btnAdd" runat="server" Text="Add Mapping" CssClass="btn btn-success" OnClick="btnAdd_Click" />
</div>

            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success font-weight-bold"></asp:Label>
        </asp:Panel>


        <!-- GridView -->
        <asp:GridView ID="gvMappings" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered mt-4"
            DataKeyNames="Inst_Code,Program_Code"
            OnRowEditing="gvMappings_RowEditing"
            OnRowUpdating="gvMappings_RowUpdating"
            OnRowCancelingEdit="gvMappings_RowCancelingEdit">
            <Columns>
                <asp:BoundField DataField="Program_Code" HeaderText="Program" ReadOnly="True" />
                <asp:BoundField DataField="Affiliation_Year" HeaderText="Affiliation Year" />
                <asp:BoundField DataField="Current_Approved_Intake" HeaderText="Approved Intake" />
                <asp:CheckBoxField DataField="Is_Active" HeaderText="Active" />
                <asp:CheckBoxField DataField="Is_Aided" HeaderText="Aided" />
                <asp:CommandField ShowEditButton="True" />
            </Columns>
        </asp:GridView>
    </div>

                 </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>

        