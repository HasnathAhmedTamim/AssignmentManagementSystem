import { getToken, clearAuth } from "./auth-storage";
import type {
  Assignment,
  AuthUser,
  ClassRoom,
  CreateAssignmentRequest,
  CreateSubmissionRequest,
  CreateUserRequest,
  Enrollment,
  GradeSubmissionRequest,
  LoginResponse,
  Subject,
  Submission,
  TeacherAssignment,
  UpdateAssignmentRequest,
  UpdateSubmissionRequest,
  UpdateUserRequest,
  User,
} from "./types";

const DEFAULT_API_URL = "http://localhost:5249/api";

export function getApiBaseUrl(): string {
  return process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "") || DEFAULT_API_URL;
}

export class ApiError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

async function parseError(res: Response): Promise<string> {
  try {
    const data = (await res.json()) as {
      message?: string;
      Message?: string;
      title?: string;
      errors?: Record<string, string[]>;
    };
    if (data.message) return data.message;
    if (data.Message) return data.Message;
    if (data.errors) {
      const first = Object.values(data.errors).flat()[0];
      if (first) return first;
    }
    if (data.title) return data.title;
  } catch {
    // ignore
  }
  return res.statusText || "Request failed";
}

async function request<T>(
  path: string,
  options: RequestInit = {},
  auth = true
): Promise<T> {
  const headers = new Headers(options.headers);
  if (!headers.has("Content-Type") && options.body) {
    headers.set("Content-Type", "application/json");
  }

  if (auth) {
    const token = getToken();
    if (token) headers.set("Authorization", `Bearer ${token}`);
  }

  const res = await fetch(`${getApiBaseUrl()}${path}`, {
    ...options,
    headers,
  });

  if (res.status === 401 && auth) {
    clearAuth();
    if (typeof window !== "undefined" && !window.location.pathname.startsWith("/login")) {
      window.location.href = "/login";
    }
  }

  if (!res.ok) {
    throw new ApiError(await parseError(res), res.status);
  }

  if (res.status === 204) {
    return undefined as T;
  }

  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text) as T;
}

export const api = {
  login(email: string, password: string) {
    return request<LoginResponse>(
      "/auth/login",
      {
        method: "POST",
        body: JSON.stringify({ email, password }),
      },
      false
    );
  },

  // Users
  getUsers() {
    return request<User[]>("/users");
  },
  createUser(data: CreateUserRequest) {
    return request<User>("/users", { method: "POST", body: JSON.stringify(data) });
  },
  updateUser(id: string, data: UpdateUserRequest) {
    return request<void>(`/users/${id}`, { method: "PUT", body: JSON.stringify(data) });
  },
  deleteUser(id: string) {
    return request<void>(`/users/${id}`, { method: "DELETE" });
  },

  // ClassRooms
  getClassRooms() {
    return request<ClassRoom[]>("/classRooms");
  },
  createClassRoom(data: { name: string; section: string }) {
    return request<ClassRoom>("/classRooms", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  updateClassRoom(id: string, data: { name: string; section: string }) {
    return request<void>(`/classRooms/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },
  deleteClassRoom(id: string) {
    return request<void>(`/classRooms/${id}`, { method: "DELETE" });
  },

  // Subjects
  getSubjects() {
    return request<Subject[]>("/subjects");
  },
  createSubject(data: { name: string; code: string }) {
    return request<Subject>("/subjects", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  updateSubject(id: string, data: { name: string; code: string }) {
    return request<void>(`/subjects/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },
  deleteSubject(id: string) {
    return request<void>(`/subjects/${id}`, { method: "DELETE" });
  },

  // Teacher assignments
  getTeacherAssignments(teacherId?: string) {
    const q = teacherId ? `?teacherId=${teacherId}` : "";
    return request<TeacherAssignment[]>(`/teacher-assignments${q}`);
  },
  createTeacherAssignment(data: {
    teacherId: string;
    classRoomId: string;
    subjectId: string;
  }) {
    return request<TeacherAssignment>("/teacher-assignments", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  deleteTeacherAssignment(id: string) {
    return request<void>(`/teacher-assignments/${id}`, { method: "DELETE" });
  },

  // Enrollments
  getEnrollments(studentId?: string) {
    const q = studentId ? `?studentId=${studentId}` : "";
    return request<Enrollment[]>(`/enrollments${q}`);
  },
  createEnrollment(data: { studentId: string; classRoomId: string }) {
    return request<Enrollment>("/enrollments", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  deleteEnrollment(id: string) {
    return request<void>(`/enrollments/${id}`, { method: "DELETE" });
  },

  // Assignments
  getAssignments() {
    return request<Assignment[]>("/assignments");
  },
  getAssignment(id: string) {
    return request<Assignment>(`/assignments/${id}`);
  },
  createAssignment(data: CreateAssignmentRequest) {
    return request<Assignment>("/assignments", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  updateAssignment(id: string, data: UpdateAssignmentRequest) {
    return request<void>(`/assignments/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },
  publishAssignment(id: string) {
    return request<void>(`/assignments/${id}/publish`, { method: "POST" });
  },
  deleteAssignment(id: string) {
    return request<void>(`/assignments/${id}`, { method: "DELETE" });
  },

  // Submissions
  getSubmissionsByAssignment(assignmentId: string) {
    return request<Submission[]>(`/submissions/assignment/${assignmentId}`);
  },
  getMySubmissions() {
    return request<Submission[]>("/submissions/mine");
  },
  getSubmission(id: string) {
    return request<Submission>(`/submissions/${id}`);
  },
  createSubmission(data: CreateSubmissionRequest) {
    return request<Submission>("/submissions", {
      method: "POST",
      body: JSON.stringify(data),
    });
  },
  updateSubmission(id: string, data: UpdateSubmissionRequest) {
    return request<void>(`/submissions/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },
  gradeSubmission(id: string, data: GradeSubmissionRequest) {
    return request<void>(`/submissions/${id}/grade`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },
};

export function toAuthUser(res: LoginResponse): AuthUser {
  return {
    userId: res.userId,
    fullName: res.fullName,
    email: res.email,
    role: res.role,
  };
}
