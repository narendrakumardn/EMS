<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master" CodeBehind="ViewStudent.aspx.cs" Inherits="BTEDiploma.admin.ViewStudent" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 

    
   

    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="True" /> 
    <asp:Label ID="Label1" runat="server" ForeColor="Red" Font-Bold="True" />

    <div class="container mt-4">

     
        <h3 class="text-center text-white py-2 mb-3 rounded" 
            style="background-color:#007bff;">
            Student Details - Post approval from ACM
        </h3>

       
        <h4 class="text-center text-dark py-2 mb-4 rounded" 
            style="background-color:#f8d7da;">
            Contact Exam Section in case of change
        </h4>

       
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

                
            <div class="mb-3"> <label class="fw-bold me-2">Academic Year:</label>
        <asp:DropDownList ID="ddlAcademicYear" runat="server" Width="300px" CssClass="form-select d-inline-block" />

            </div>

                <div class="mb-3">
                    <label class="fw-bold me-2">Program:</label>
                    <asp:DropDownList ID="ddlProgram" runat="server" AutoPostBack="true" 
                        OnSelectedIndexChanged="ddlFilter_Changed" 
                        CssClass="form-select d-inline-block" />
                </div>

              
                <asp:GridView ID="gvViewStudent" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-bordered table-striped table-hover text-nowrap">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No">
    <ItemTemplate>
        <%# Container.DataItemIndex + 1 %>
    </ItemTemplate>
    <ItemStyle CssClass="text-center px-3 py-2" />
    <HeaderStyle CssClass="bg-light fw-bold text-center px-3 py-2" />
</asp:TemplateField>

                        <asp:BoundField DataField="Student_Name" HeaderText="Student Name">
                            <ItemStyle CssClass="text-wrap px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-wrap px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Register_Number" HeaderText="Register Number">
                            <ItemStyle CssClass="text-wrap px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-wrap px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="ACM_Approval_Status_Text" HeaderText="Is Approved By ACM">
                            <ItemStyle CssClass="text-center px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-center px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="CatCode" HeaderText="Category Code">
                            <ItemStyle CssClass="text-center px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-center px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Gender" HeaderText="Gender">
                            <ItemStyle CssClass="text-center px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-center px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Admission_Type_Description" HeaderText="Admission Type">
                            <ItemStyle CssClass="text-wrap px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-wrap px-3 py-2" />
                        </asp:BoundField>

                        <asp:BoundField DataField="Email" HeaderText="Email ID">
                            <ItemStyle CssClass="text-wrap px-3 py-2" />
                            <HeaderStyle CssClass="bg-light fw-bold text-wrap px-3 py-2" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>

               
                <asp:Label ID="lblResult" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>

            </ContentTemplate>
            <Triggers>
                
                <asp:AsyncPostBackTrigger ControlID="ddlAcademicYear" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="ddlProgram" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>

    </div>
</asp:Content>
