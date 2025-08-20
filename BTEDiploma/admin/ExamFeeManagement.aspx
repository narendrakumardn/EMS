<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamFeeManagement.aspx.cs" Inherits="BTEDiploma.admin.ExamFeeManagement"
    MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-white bg-primary p-3 rounded">Exam Fee Management</h2>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row g-3 mt-3">
                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Select Operation" CssClass="form-label" />
                        <asp:DropDownList ID="ddlOperation" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlOperation_SelectedIndexChanged">
                            <asp:ListItem Text="--Select--" Value="" />
                            <asp:ListItem Text="Backlog Fee" Value="Backlog" />
                            <asp:ListItem Text="Late Fine" Value="LateFine" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Academic Year" CssClass="form-label" />
                        <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Semester" CssClass="form-label" />
                        <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSemester_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <asp:Panel ID="pnlBacklog" runat="server" Visible="false">
                        <div class="col-md-6">
                            <asp:Label runat="server" Text="Backlog Count" CssClass="form-label" />
                            <asp:DropDownList ID="ddlBacklogCount" runat="server" CssClass="form-select">
                                <asp:ListItem Text="1" Value="1" />
                                <asp:ListItem Text="2" Value="2" />
                                <asp:ListItem Text="3" Value="3" />
                                <asp:ListItem Text="4" Value="4" />
                                <asp:ListItem Text="5" Value="5" />
                                <asp:ListItem Text="6" Value="6" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <asp:Label runat="server" Text="Backlog Fee (INR)" CssClass="form-label" />
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="Enter amount in INR" TextMode="Number"></asp:TextBox>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlLateFine" runat="server" Visible="false">
                        <div class="col-md-6">
                            <asp:Label runat="server" Text="Last Date" CssClass="form-label" />
                            <asp:TextBox ID="txtLastDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <asp:Label runat="server" Text="Late Fine (INR)" CssClass="form-label" />
                            <asp:TextBox ID="txtLateFine" runat="server" CssClass="form-control" placeholder="Enter amount in INR" TextMode="Number"></asp:TextBox>
                        </div>
                    </asp:Panel>

                    <div class="col-md-12 text-center mt-3">
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary px-4" Text="Submit" OnClick="btnSubmit_Click" />
                    </div>
                </div>

                <hr class="my-4" />

                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped"
                    DataKeyNames="Exam_ID"
                    OnRowEditing="gvData_RowEditing"
                    OnRowCancelingEdit="gvData_RowCancelingEdit"
                    OnRowUpdating="gvData_RowUpdating">
                    
                    <Columns>
                        <asp:TemplateField HeaderText="Backlog Count / Last Date">
                            <ItemTemplate>
                                <asp:Label ID="lblValue" runat="server" Text='<%# Eval("ValueField") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditValue" runat="server" Text='<%# Bind("ValueField") %>' CssClass="form-control"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fee / Late Fine">
                            <ItemTemplate>
                                <asp:Label ID="lblFee" runat="server" Text='<%# Eval("FeeField") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditFee" runat="server" Text='<%# Bind("FeeField") %>' CssClass="form-control"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="True" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
