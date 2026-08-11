export type Role = "Admin" | "Teacher" | "Student";
export type AssignmentStatus = "Draft" | "Published";
export type SubmissionStatus = "Pending" | "Reviewed" | "Late";

export interface AuthUser {
  userId: string;
  fullName: string;
  email: string;
  role: Role;
}

export interface LoginResponse {
  token: string;
  userId: string;
  fullName: string;
  email: string;
  role: Role;
}

export interface User {
  id: string;
  fullName: string;
  email: string;
  role: Role;
  isActive: boolean;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: Role;
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  role: Role;
  isActive: boolean;
}

export interface ClassRoom {
  id: string;
  name: string;
  section: string;
  createdAt: string;
}

export interface Subject {
  id: string;
  name: string;
  code: string;
  createdAt: string;
}

export interface TeacherAssignment {
  id: string;
  teacherId: string;
  teacherName: string;
  classRoomId: string;
  classRoomName: string;
  classRoomSection: string;
  subjectId: string;
  subjectName: string;
  subjectCode: string;
  createdAt: string;
}

export interface Enrollment {
  id: string;
  studentId: string;
  studentName: string;
  classRoomId: string;
  classRoomName: string;
  classRoomSection: string;
  createdAt: string;
}

export interface StudentSubmissionSummary {
  id: string;
  status: SubmissionStatus;
  marks: number | null;
  submittedAt: string;
  canUpdate: boolean;
}

export interface Assignment {
  id: string;
  title: string;
  description: string;
  deadline: string;
  maximumMarks: number;
  status: AssignmentStatus;
  teacherClassSubjectId: string;
  classRoomName: string;
  subjectName: string;
  teacherName: string;
  createdAt: string;
  mySubmission?: StudentSubmissionSummary | null;
}

export interface CreateAssignmentRequest {
  teacherClassSubjectId: string;
  title: string;
  description: string;
  deadline: string;
  maximumMarks: number;
}

export interface UpdateAssignmentRequest {
  title: string;
  description: string;
  deadline: string;
  maximumMarks: number;
}

export interface Submission {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  studentId: string;
  studentName: string;
  answer: string;
  submittedAt: string;
  marks: number | null;
  feedback: string | null;
  status: SubmissionStatus;
  canUpdate: boolean;
}

export interface CreateSubmissionRequest {
  assignmentId: string;
  answer: string;
}

export interface UpdateSubmissionRequest {
  answer: string;
}

export interface GradeSubmissionRequest {
  marks: number;
  feedback?: string | null;
  status: SubmissionStatus;
}
