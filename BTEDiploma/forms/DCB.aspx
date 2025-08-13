<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DCB.aspx.cs" Inherits="BTEDiploma.forms.DCB"  MasterPageFile="~/Site1.Master"%>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <h2 class="text-primary">College DCB Dashboard</h2>
        <hr />

        <asp:Label ID="lblCollege" runat="server" CssClass="h5 text-success"></asp:Label>
        <asp:Label ID="lblStudents" runat="server" CssClass="h6 text-muted"></asp:Label>
        <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

        <div class="row text-center my-4">
            <div class="col-md-4">
                <div class="card border-primary">
                    <div class="card-body">
                        <h5 class="text-primary">Total Demand</h5>
                        <h3><asp:Label ID="lblDemand" runat="server" Text="0" /></h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card border-success">
                    <div class="card-body">
                        <h5 class="text-success">Total Collection</h5>
                        <h3><asp:Label ID="lblCollection" runat="server" Text="0" /></h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card border-warning">
                    <div class="card-body">
                        <h5 class="text-warning">Total Balance</h5>
                        <h3><asp:Label ID="lblBalance" runat="server" Text="0" /></h3>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-8">
                <asp:GridView ID="gvBranchDCB" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-hover">
                    <Columns>
                        <asp:BoundField DataField="BranchCode" HeaderText="Branch Code" />
                        <asp:BoundField DataField="BranchName" HeaderText="Branch Name" />
                        <asp:BoundField DataField="StudentCount" HeaderText="Students" />
                        <asp:BoundField DataField="Demand" HeaderText="Demand" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Collection" HeaderText="Collection" DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
            <div class="col-md-4">
                <canvas id="dcbPieChart" width="300" height="300"></canvas>
            </div>
        </div>
    </div>

    <script>
        function updateChart(demand, collection, balance) {
            const ctx = document.getElementById('dcbPieChart').getContext('2d');
            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Collection', 'Balance'],
                    datasets: [{
                        data: [collection, balance],
                        backgroundColor: ['#28a745', '#ffc107']
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    },
                    cutout: '70%'
                }
            });
        }
    </script>
</asp:Content>
