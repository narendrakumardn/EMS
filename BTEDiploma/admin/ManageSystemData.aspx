
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageSystemData.aspx.cs" Inherits="BTEDiploma.admin.ManageSystemData" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container mt-4">
    <h3 class="mb-4">   Manage System Data</h3>
    <div class="row g-4">

        <!-- Panel 1: Academics -->
        <div class="col-md-4">
            <div class="card border-primary">
                <div class="card-header bg-primary text-white">
                    <i class="fas fa-university me-1"></i> Academics
                </div>
                <div class="card-body">
                    <ul class="list-unstyled">
                        <li><a href="Institute.aspx"><i class="fas fa-link me-2"></i> Add Institution</a></li>
                        <li><a href="InstitutionToProgramMapping.aspx"><i class="fas fa-link me-2"></i> Institution–Program Mapping</a></li>
                        <li><a href="ProgramToCourseMapping.aspx"><i class="fas fa-project-diagram me-2"></i> Program–Course Mapping</a></li>
                       <li> <a href="PCMapping11.aspx"><i class="fas fa-project-diagram me-2"></i> Program–Course Mapping
                    </a>
                </li>

                    </ul>
                </div>
            </div>
        </div>

      <!-- Panel 2: Curriculum -->
        <div class="col-md-4">
            <div class="card border-success">
                <div class="card-header bg-success text-white">
                    <i class="fas fa-book me-1"></i> Curriculum
                </div>
                <div class="card-body">
                    <ul class="list-unstyled">
                        <li><a href="AcademicYear.aspx"><i class="fas fa-calendar-alt me-2"></i> Academic Year</a></li>
                        <li><a href="CreateCurriculumScheme.aspx"><i class="fas fa-file-alt me-2"></i> Create Curriculum Scheme</a></li>
                        <li><a href="CreateProgram.aspx"><i class="fas fa-file-alt me-2"></i> Create Program</a></li>
                        <li><a href="Courses.aspx"><i class="fas fa-file-alt me-2"></i> Create Course</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <!-- Panel 3: Exam Related -->
        <div class="col-md-4">
            <div class="card border-warning">
                <div class="card-header bg-warning text-dark">
                    <i class="fas fa-clipboard-list me-1"></i> Exam Related
                </div>
                <div class="card-body">
                    <ul class="list-unstyled">
                         <li><a href="ExamCreation.aspx"><i class="fas fa-clock me-2"></i> Create Exam </a></li>
                        <li><a href="ExamFeeManagement.aspx"><i class="fas fa-clock me-2"></i> Add Backlog Count,Fee and Late Fine </a></li>
                        <li><a href="ExamTimetableadd.aspx"><i class="fas fa-clock me-2"></i> Exam Timetable</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <!-- Optional Panel 4: Future Use -->
        <div class="col-md-4">
            <div class="card border-secondary">
                <div class="card-header bg-secondary text-white">
                    <i class="fas fa-database me-1"></i> Reserved Panel
                </div>
                <div class="card-body">
                    <p>Future data or configuration can be added here.</p>
                </div>
            </div>
        </div>

    </div>
</div>
</asp:Content>
