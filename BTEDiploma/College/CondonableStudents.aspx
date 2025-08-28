<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master" CodeBehind="CondonableStudents.aspx.cs" Inherits="BTEDiploma.admin.CondonableStudents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- If ScriptManager is already in Site1.master, remove this line -->
    

    <!-- Optional: small spinner during async postbacks -->
    <asp:UpdateProgress ID="updProg" runat="server" AssociatedUpdatePanelID="updMain" DisplayAfter="150">
        <ProgressTemplate>
            <div class="position-fixed top-0 start-50 translate-middle-x mt-3 alert alert-info py-2 px-3 shadow">
                Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>

            <div class="container mt-4">
                <h3 class="mb-1">Condonable Student Details Entry</h3>
                <h5 class="text-muted mb-3">Contact Exam Section in case of change in the list</h5>

                <!-- Error / Info Messages -->
                <div class="text-center my-3">
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" CssClass="mb-2"></asp:Label>
                    <asp:Label ID="lblResult" runat="server" ForeColor="Green" CssClass="mb-2"></asp:Label>

                    <asp:Panel ID="PnlViewReport" runat="server" Visible="false" CssClass="p-3 border rounded bg-light">
                        <asp:Label ID="lblStatus" runat="server" CssClass="fw-bold text-success d-block mb-2" Visible="false"></asp:Label>
                        <asp:Button ID="btnViewReport" runat="server" Text="View Final Report and Download"
                                    CssClass="btn btn-outline-primary"
                                    OnClick="btnViewReport_Click" />
                    </asp:Panel>
                </div>

                <!-- Condonable Panel -->
                <asp:Panel ID="pnlCondonable" runat="server" Visible="false" CssClass="p-3 border rounded bg-light">

                    <asp:Panel ID="PnlEditCondonable" runat="server" Visible="false" CssClass="p-3 border rounded bg-light">
                        <!-- Dropdowns Row -->
                        <div class="row mb-3 d-flex justify-content-center">
                            <div class="col-md-4">
                                <asp:Label ID="lblStudent" runat="server" Text="Select Student:" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlCondonableStudents" runat="server" CssClass="form-select"
                                                  AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Cond_Student_Changed">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <asp:Label ID="lblCourse" runat="server" Text="Select Course:" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlCondonableCourses" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <asp:Label ID="lblReason" runat="server" Text="Condonable Reason:" CssClass="form-label"></asp:Label>
                                <asp:DropDownList ID="ddlCondonableReason" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                        </div>

                      
                        <div class="row mb-3 d-flex justify-content-center">
                            <div class="col-md-4">
                                <asp:Label ID="lblDate" runat="server" Text="Date:" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtCondonableDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-8">
                                <asp:Label ID="lblRemarks" runat="server" Text="Remarks:" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="text-center mb-3">
                            <asp:Button ID="btnSaveCondonable" runat="server" Text="Save Condonable Student"
                                        CssClass="btn btn-success" OnClick="btnSaveCondonable_Click" />
                        </div>

                        <hr />
                    </asp:Panel>

              
                    <asp:GridView ID="gvCondonableList" runat="server" CssClass="table table-bordered table-striped"
                                  AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Student Name" />
                            <asp:BoundField DataField="Register_Number" HeaderText="Register Number" />
                            <asp:BoundField DataField="Course_Code" HeaderText="Course Code" />
                            <asp:BoundField DataField="Course_Name" HeaderText="Course Name" />
                            <asp:BoundField DataField="Reason_Desc" HeaderText="Reason" />
                            <asp:BoundField DataField="DateofCertificate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                            <asp:BoundField DataField="Remark" HeaderText="Remarks" />
                        </Columns>
                    </asp:GridView>

                    <asp:Button ID="btnSubmitCondList" runat="server" Text="Submit (Finalize)" CssClass="btn btn-success"
                                OnClick="btnSubmit_ClickCondList" />
                </asp:Panel>
            </div>

        </ContentTemplate>

       
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnViewReport" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ddlCondonableStudents" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveCondonable" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSubmitCondList" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
