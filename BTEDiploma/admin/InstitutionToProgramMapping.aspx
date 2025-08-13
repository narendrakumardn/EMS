<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"  CodeBehind="InstitutionToProgramMapping.aspx.cs" Inherits="BTEDiploma.admin.InstitutionToProgramMapping" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h3>Institution to Program Mapping</h3>

        <div class="form-group">
            <label>Select Institution</label>
            <asp:DropDownList ID="ddlInstitution" runat="server" AutoPostBack="true"
                OnSelectedIndexChanged="ddlInstitution_SelectedIndexChanged" CssClass="form-control" />
        </div>


        <asp:Panel ID="pnlAddNew" runat="server" Visible="false" CssClass="card p-3 mt-4">
            <h5>Add New Mapping</h5>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <label>Program</label>
                    <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group col-md-2">
                    <label>Affiliation Year</label>
                    <asp:TextBox ID="txtYear" runat="server" CssClass="form-control" placeholder="e.g. 2025" />
                </div>
                <div class="form-group col-md-2">
    <label>Approved Intake</label>
    <asp:DropDownList ID="ddlIntake" runat="server" CssClass="form-control">
        <asp:ListItem Text="60" Value="60" />
        <asp:ListItem Text="63" Value="63" />
        <asp:ListItem Text="40" Value="40" />
        <asp:ListItem Text="30" Value="30" />
    </asp:DropDownList>
</div>

                <div class="form-group col-md-2">
                    <label>&nbsp;</label>
                    <asp:CheckBox ID="chkActive" runat="server" Text="Active" CssClass="form-control" />
                </div>
                <div class="form-group col-md-2">
    <label>&nbsp;</label>
    <asp:CheckBox ID="chkAided" runat="server" Text="Government/Aided" CssClass="form-control" />
</div>
                <div class="form-group col-md-2">
                    <label>&nbsp;</label>
                    <asp:Button ID="btnAdd" runat="server" Text="Add Mapping" CssClass="btn btn-success form-control" OnClick="btnAdd_Click" />
                </div>
            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success"></asp:Label>
        </asp:Panel>

        
  <asp:GridView ID="gvMappings" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered mt-3"
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
</asp:Content>