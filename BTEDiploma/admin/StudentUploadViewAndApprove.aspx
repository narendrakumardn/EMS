<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"  
    CodeBehind="StudentUploadViewAndApprove.aspx.cs" 
    Inherits="BTEDiploma.admin.StudentUploadViewAndApprove" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

    <div class="container mt-4">

   
        <h3 class="bg-primary text-white p-2 rounded">Approve Student Details - ACM</h3>
        <div class="alert alert-warning mt-2">
            ⚠️ Candidates without approval from ACM should be deselected here.
        </div>

       
        <asp:UpdatePanel ID="updPanel" runat="server">
            <ContentTemplate>

                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-striped table-hover table-bordered table-sm align-middle"
                    HeaderStyle-CssClass="table-dark text-center"
                    RowStyle-CssClass="text-justify"
                    AlternatingRowStyle-CssClass="table-light"
                    DataKeyNames="KeyValue"
                    OnRowCommand="gvData_RowCommand"
                    OnRowDataBound="gvData_RowDataBound">

                    <Columns>
                    
                        <asp:TemplateField HeaderText="Sl. No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-center" Width="60px" />
                        </asp:TemplateField>

                        <asp:BoundField DataField="Student_Name" HeaderText="Student Name" />
                        <asp:BoundField DataField="Register_Number" HeaderText="Register Number" />

                       
                        <asp:TemplateField HeaderText="Approve ACM">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkApprove" runat="server" Checked='<%# Eval("IsApproved") %>' />
                            </ItemTemplate>
                            <ItemStyle CssClass="text-center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:Button ID="btnApprove" runat="server" Text="Approve Selected" 
                    CssClass="btn btn-success mt-2" OnClick="btnApprove_Click" Visible="false" />

                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block mt-2" Visible="false" />

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnApprove" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
