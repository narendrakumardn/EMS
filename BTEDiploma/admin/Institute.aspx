<%@ Page Title="Add / Update Institute" Language="C#" MasterPageFile="~/Site1.Master"
    AutoEventWireup="true" CodeBehind="Institute.aspx.cs" Inherits="BTEDiploma.admin.Institute"
    UnobtrusiveValidationMode="None" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <div class="container mt-5" style="min-height: 600px;">
        <h2 class="text-center text-primary mb-4">Add / Update Institute</h2>

        <!-- Show all validation errors -->
        <asp:ValidationSummary ID="vsMain" runat="server" CssClass="alert alert-danger"
            ValidationGroup="Institute" HeaderText="Please fix the following:" />

        <div class="row g-3 mb-4">

            <!-- Institute Code -->
            <div class="col-md-4">
                <label>Institute Code</label>
                <asp:TextBox ID="TxtInst" runat="server" CssClass="form-control" MaxLength="20" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtInst"
                    ErrorMessage="Institute Code is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtInst"
                    ValidationExpression="^[A-Za-z0-9\-]+$"
                    ErrorMessage="Institute Code can contain letters, numbers, and dashes only"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Institute Name -->
            <div class="col-md-4">
                <label>Institute Name</label>
                <asp:TextBox ID="TxtInstName" runat="server" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtInstName"
                    ErrorMessage="Institute Name is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtInstName"
                    ValidationExpression=".{3,}" ErrorMessage="Institute Name must be at least 3 characters"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Institute Type -->
            <div class="col-md-4">
                <label>Institute Type</label>
                <asp:DropDownList ID="ddlinsttype" runat="server" CssClass="form-control"
                    AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Type --" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlinsttype" InitialValue=""
                    ErrorMessage="Institute Type is required" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- AICTE ID -->
            <div class="col-md-4">
                <label>AICTE ID</label>
                <asp:TextBox ID="TxtAicte" runat="server" CssClass="form-control" MaxLength="50" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtAicte"
                    ErrorMessage="AICTE ID is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtAicte"
                    ValidationExpression="^[A-Za-z0-9\-\/]+$"
                    ErrorMessage="AICTE ID can contain letters, numbers, dashes, and slashes"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Shift -->
            <div class="col-md-4">
                <label>Shift</label>
                <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control"
                    AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Shift --" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlShift" InitialValue=""
                    ErrorMessage="Shift is required" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Gender -->
            <div class="col-md-4">
                <label>Gender</label>
                <asp:DropDownList ID="ddlgender" runat="server" CssClass="form-control"
                    AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Gender --" Value="" />
                    <asp:ListItem Text="CO-ED" Value="C" />
                    <asp:ListItem Text="Girls" Value="W" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlgender" InitialValue=""
                    ErrorMessage="Gender is required" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Taluk -->
            <div class="col-md-4">
                <label>Taluk</label>
                <asp:DropDownList ID="ddltaluk" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Taluk --" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddltaluk" InitialValue=""
                    ErrorMessage="Taluk is required" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Is Exam Center -->
            <div class="col-md-4">
                <label>Is Exam Center</label>
                <asp:DropDownList ID="ddlexamcenter" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Option --" Value="" />
                    <asp:ListItem Text="YES" Value="1" />
                    <asp:ListItem Text="NO" Value="0" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlexamcenter" InitialValue=""
                    ErrorMessage="Please select if it is an Exam Center" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Exam Zone -->
            <div class="col-md-4">
                <label>Exam Zone</label>
                <asp:DropDownList ID="ddlExamzone" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Select Exam Zone --" Value="" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlExamzone" InitialValue=""
                    ErrorMessage="Exam Zone is required" CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Address -->
            <div class="col-md-4">
                <label>Address</label>
                <asp:TextBox ID="TxtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="300" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtAddress"
                    ErrorMessage="Address is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtAddress"
                    ValidationExpression=".{5,}"
                    ErrorMessage="Address must be at least 5 characters"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Pincode -->
            <div class="col-md-4">
                <label for="Txtpincode">Pincode</label>
                <asp:TextBox ID="Txtpincode" runat="server" CssClass="form-control" MaxLength="6" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Txtpincode"
                    ErrorMessage="Pincode is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="Txtpincode"
                    ValidationExpression="^[1-9][0-9]{5}$"
                    ErrorMessage="Enter a valid 6-digit Indian pincode"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Principal ID -->
            <div class="col-md-4">
                <label>Principal ID</label>
                <asp:TextBox ID="TxtPrincipal" runat="server" CssClass="form-control" MaxLength="10" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPrincipal"
                    ErrorMessage="Principal ID is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtPrincipal"
                    ValidationExpression="^\d+$"
                    ErrorMessage="Principal ID must be numeric"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Email -->
            <div class="col-md-4">
                <label>Email</label>
                <asp:TextBox ID="Txtemail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Txtemail"
                    ErrorMessage="Email is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="Txtemail"
                    ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
                    ErrorMessage="Enter a valid email address"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>

            <!-- Phone -->
            <div class="col-md-4">
                <label>Phone</label>
                <asp:TextBox ID="Txtphone" runat="server" CssClass="form-control" TextMode="Phone" MaxLength="10" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Txtphone"
                    ErrorMessage="Phone is required" CssClass="text-danger" ValidationGroup="Institute" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="Txtphone"
                    ValidationExpression="^[6-9]\d{9}$"
                    ErrorMessage="Enter a valid 10-digit Indian mobile (starts with 6-9)"
                    CssClass="text-danger" ValidationGroup="Institute" />
            </div>
        </div>

        <asp:Label ID="Label1" runat="server" CssClass="text-danger"></asp:Label>
         <asp:Button ID="btnSave" runat="server" Text="Save Institute" CssClass="btn btn-success mb-3"
             OnClick="btnSave_Click" ValidationGroup="Institute" CausesValidation="true" />
         <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary mb-3 ms-2"
             CausesValidation="false"   OnClick="btnCancel_Click" />

         <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
        <!-- GridView (add validators in EditItemTemplates; separate group RowEdit) -->
        <asp:GridView ID="gvInstitute" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered mt-3"
            DataKeyNames="Inst_Code"
            OnRowEditing="gvInstitute_RowEditing"
            OnRowUpdating="gvInstitute_RowUpdating"
            OnRowCancelingEdit="gvInstitute_RowCancelingEdit"
            OnRowDeleting="gvInstitute_RowDeleting"
            OnRowDataBound="gvInstitute_RowDataBound">

            <Columns>
                <asp:TemplateField HeaderText="Institution Code">
                    <ItemTemplate><%# Eval("Inst_Code") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtInstCode" runat="server" Text='<%# Eval("Inst_Code") %>' ReadOnly="true" CssClass="form-control" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Institution Name">
                    <ItemTemplate><%# Eval("Inst_Name") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtInstName" runat="server" Text='<%# Eval("Inst_Name") %>' CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtInstName"
                            ErrorMessage="Name required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtInstName"
                            ValidationExpression=".{3,}" ErrorMessage="Min 3 chars"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Taluk">
                    <ItemTemplate><%# Eval("Taluk_Name") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlTaluk" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- Select Taluk --" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTaluk" InitialValue=""
                            ErrorMessage="Taluk required" CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Address">
                    <ItemTemplate><%# Eval("Address") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtAddress" runat="server" Text='<%# Eval("Address") %>' CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAddress"
                            ErrorMessage="Address required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAddress"
                            ValidationExpression=".{5,}" ErrorMessage="Min 5 chars"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Pincode">
                    <ItemTemplate><%# Eval("Pincode") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPincode" runat="server" Text='<%# Eval("Pincode") %>' CssClass="form-control" MaxLength="6" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPincode"
                            ErrorMessage="Pincode required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPincode"
                            ValidationExpression="^[1-9][0-9]{5}$" ErrorMessage="Invalid pincode"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Institution Type">
                    <ItemTemplate><%# Eval("Institution_Type_Description") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlInstitutionType" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- Select Type --" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlInstitutionType" InitialValue=""
                            ErrorMessage="Type required" CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Shift">
                    <ItemTemplate><%# Eval("Shift_Name") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- Select Shift --" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlShift" InitialValue=""
                            ErrorMessage="Shift required" CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Exam Zone">
                    <ItemTemplate><%# Eval("Exam_Zone_Name") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlExamZone" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- Select Exam Zone --" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlExamZone" InitialValue=""
                            ErrorMessage="Exam Zone required" CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="AICTE ID">
                    <ItemTemplate><%# Eval("AICTE_ID") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtAICTEID" runat="server" Text='<%# Eval("AICTE_ID") %>' CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAICTEID"
                            ErrorMessage="AICTE ID required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAICTEID"
                            ValidationExpression="^[A-Za-z0-9\-\/]+$" ErrorMessage="Letters/numbers/-/ only"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Exam Center?">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkExamCenter" runat="server" Checked='<%# Convert.ToBoolean(Eval("Is_Exam_Center")) %>' Enabled="false" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <!-- Keep checkbox but also bind a hidden dropdown if you need strict required -->
                        <asp:CheckBox ID="chkIsExamCenter" runat="server" Checked='<%# Convert.ToBoolean(Eval("Is_Exam_Center")) %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Gender">
                    <ItemTemplate><%# Eval("Gender") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                            <asp:ListItem Text="-- Select Gender --" Value="" />
                            <asp:ListItem Text="CO-ED" Value="C" />
                            <asp:ListItem Text="Girls" Value="W" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlGender" InitialValue=""
                            ErrorMessage="Gender required" CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Principal ID">
                    <ItemTemplate><%# Eval("Principal_ID") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPrincipalID" runat="server" Text='<%# Eval("Principal_ID") %>' CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPrincipalID"
                            ErrorMessage="Principal ID required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPrincipalID"
                            ValidationExpression="^\d+$" ErrorMessage="Numeric only"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate><%# Eval("Email_ID") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEmail" runat="server" Text='<%# Eval("Email_ID") %>' CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                            ErrorMessage="Email required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                            ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$" ErrorMessage="Invalid email"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Phone">
                    <ItemTemplate><%# Eval("Phone") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPhone" runat="server" Text='<%# Eval("Phone") %>' CssClass="form-control" MaxLength="10" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone"
                            ErrorMessage="Phone required" CssClass="text-danger" ValidationGroup="RowEdit" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPhone"
                            ValidationExpression="^[6-9]\d{9}$" ErrorMessage="Invalid Indian mobile"
                            CssClass="text-danger" ValidationGroup="RowEdit" />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>

       
    </div>
</asp:Content>