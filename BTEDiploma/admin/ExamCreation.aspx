<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamCreation.aspx.cs" Inherits="BTEDiploma.admin.ExamCreation"
    MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-white bg-primary p-3 rounded">Exam Creation</h2>

        <asp:UpdatePanel ID="updPanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Message placeholder for Bootstrap alerts -->
                <div class="row">
                    <div class="col-12">
                        <asp:PlaceHolder ID="phMessage" runat="server" Visible="false"></asp:PlaceHolder>
                    </div>
                </div>

                <div class="row g-3 mt-3">
                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Academic Year" CssClass="form-label" />
                        <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Year" CssClass="form-label" />
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Semester" CssClass="form-label" />
                        <asp:DropDownList ID="ddlSem" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Month" CssClass="form-label" />
                        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                            <asp:ListItem Text="January" Value="1" />
                            <asp:ListItem Text="February" Value="2" />
                            <asp:ListItem Text="March" Value="3" />
                            <asp:ListItem Text="April" Value="4" />
                            <asp:ListItem Text="May" Value="5" />
                            <asp:ListItem Text="June" Value="6" />
                            <asp:ListItem Text="July" Value="7" />
                            <asp:ListItem Text="August" Value="8" />
                            <asp:ListItem Text="September" Value="9" />
                            <asp:ListItem Text="October" Value="10" />
                            <asp:ListItem Text="November" Value="11" />
                            <asp:ListItem Text="December" Value="12" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Start Date" CssClass="form-label" />
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="End Date" CssClass="form-label" />
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Regular Exam Fee" CssClass="form-label" />
                        <asp:TextBox ID="txtRegularFee" runat="server" CssClass="form-control" TextMode="Number" placeholder="Enter amount in INR" />
                    </div>

                    <div class="col-md-6">
                        <asp:Label runat="server" Text="Due Date" CssClass="form-label" />
                        <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>

                    <div class="col-md-12 text-center mt-3">
                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary px-4" Text="Submit" OnClick="btnSubmit_Click" />
                    </div>
                </div>

                <hr class="my-4" />

                <asp:GridView ID="gvExams" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped"
                    DataKeyNames="Exam_ID"
                    OnRowEditing="gvExams_RowEditing"
                    OnRowCancelingEdit="gvExams_RowCancelingEdit"
                    OnRowUpdating="gvExams_RowUpdating"
                    OnRowDataBound="gvExams_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Exam_ID" HeaderText="Exam ID" ReadOnly="True" />

                        <asp:TemplateField HeaderText="Academic Year">
                            <ItemTemplate>
                                <%# Eval("Academic_Year") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlAcademicYearEdit" runat="server" CssClass="form-select"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Month">
                            <ItemTemplate>
                                <%# GetMonthName(Eval("Exam_Month")) %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlMonthEdit" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="January" Value="1" />
                                    <asp:ListItem Text="February" Value="2" />
                                    <asp:ListItem Text="March" Value="3" />
                                    <asp:ListItem Text="April" Value="4" />
                                    <asp:ListItem Text="May" Value="5" />
                                    <asp:ListItem Text="June" Value="6" />
                                    <asp:ListItem Text="July" Value="7" />
                                    <asp:ListItem Text="August" Value="8" />
                                    <asp:ListItem Text="September" Value="9" />
                                    <asp:ListItem Text="October" Value="10" />
                                    <asp:ListItem Text="November" Value="11" />
                                    <asp:ListItem Text="December" Value="12" />
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Year">
                            <ItemTemplate>
                                <%# Eval("Exam_Year") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtExamYearEdit" runat="server" CssClass="form-control"
                                             Text='<%# Bind("Exam_Year") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Semester">
                            <ItemTemplate>
                                <%# Eval("Semester") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlSemEdit" runat="server" CssClass="form-select"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Start Date">
                            <ItemTemplate>
                                <%# Eval("Academic_Start_Date", "{0:yyyy-MM-dd}") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtStartDateEdit" runat="server" CssClass="form-control"
                                             Text='<%# Bind("Academic_Start_Date", "{0:yyyy-MM-dd}") %>' TextMode="Date" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="End Date">
                            <ItemTemplate>
                                <%# Eval("Academic_End_Date", "{0:yyyy-MM-dd}") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEndDateEdit" runat="server" CssClass="form-control"
                                             Text='<%# Bind("Academic_End_Date", "{0:yyyy-MM-dd}") %>' TextMode="Date" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Fee">
                            <ItemTemplate>
                                <%# Eval("Regular_Fee") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFeeEdit" runat="server" CssClass="form-control"
                                             Text='<%# Bind("Regular_Fee") %>' TextMode="Number" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Due Date">
                            <ItemTemplate>
                                <%# Eval("Due_Date", "{0:yyyy-MM-dd}") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDueDateEdit" runat="server" CssClass="form-control"
                                             Text='<%# Bind("Due_Date", "{0:yyyy-MM-dd}") %>' TextMode="Date" />
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="True" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlAcademicYear" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
