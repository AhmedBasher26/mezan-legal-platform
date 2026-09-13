import type {
  Activity, Case, CaseFile, CaseNote, Client, Hearing,
  ImportantDate, Lawyer, Task, Template, Tx
} from "../types";

const TOKEN_KEY = "mezan_jwt_token";

export const getToken = (): string | null => localStorage.getItem(TOKEN_KEY);
export const setToken = (token: string | null) => {
  if (token) localStorage.setItem(TOKEN_KEY, token);
  else localStorage.removeItem(TOKEN_KEY);
};

const BASE_URL = "/api";

async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorMsg = `Request failed: ${response.status}`;
    try {
      const data = await response.json();
      if (data && data.message) errorMsg = data.message;
    } catch {
      // no json response
    }
    throw new Error(errorMsg);
  }

  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
}

export const api = {
  auth: {
    login: async (identifier: string, password: string) => {
      const res = await request<{ token: string; user: Lawyer }>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ identifier, password }),
      });
      setToken(res.token);
      return res;
    },
    register: async (data: { name: string; email: string; phone: string; password: string }, withDemo: boolean) => {
      const res = await request<{ token: string; user: Lawyer }>("/auth/register", {
        method: "POST",
        body: JSON.stringify({ ...data, withDemo }),
      });
      setToken(res.token);
      return res;
    },
    me: () => request<Lawyer>("/auth/me"),
    logout: () => setToken(null),
  },

  clients: {
    list: (q?: string) => request<Client[]>(`/clients${q ? `?q=${encodeURIComponent(q)}` : ""}`),
    get: (id: string) => request<Client>(`/clients/${id}`),
    create: (data: Omit<Client, "id" | "lawyerId" | "createdAt" | "importantDates">) =>
      request<Client>("/clients", { method: "POST", body: JSON.stringify(data) }),
    update: (id: string, data: Partial<Client>) =>
      request<Client>(`/clients/${id}`, { method: "PUT", body: JSON.stringify(data) }),
    delete: (id: string) => request<void>(`/clients/${id}`, { method: "DELETE" }),
    addImportantDate: (clientId: string, label: string, date: string) =>
      request<ImportantDate>(`/clients/${clientId}/important-dates`, {
        method: "POST",
        body: JSON.stringify({ label, date }),
      }),
    deleteImportantDate: (clientId: string, dateId: string) =>
      request<void>(`/clients/${clientId}/important-dates/${dateId}`, { method: "DELETE" }),
  },

  cases: {
    list: (q?: string, status?: string) => {
      const params = new URLSearchParams();
      if (q) params.set("q", q);
      if (status && status !== "all") params.set("status", status);
      const qs = params.toString();
      return request<Case[]>(`/cases${qs ? `?${qs}` : ""}`);
    },
    get: (id: string) => request<Case>(`/cases/${id}`),
    create: (data: Omit<Case, "id" | "lawyerId" | "createdAt">) =>
      request<Case>("/cases", { method: "POST", body: JSON.stringify(data) }),
    update: (id: string, data: Partial<Case>) =>
      request<Case>(`/cases/${id}`, { method: "PUT", body: JSON.stringify(data) }),
    delete: (id: string) => request<void>(`/cases/${id}`, { method: "DELETE" }),

    getNotes: (caseId: string) => request<CaseNote[]>(`/cases/${caseId}/notes`),
    addNote: (caseId: string, text: string) =>
      request<CaseNote>(`/cases/${caseId}/notes`, { method: "POST", body: JSON.stringify({ text }) }),

    getHearings: (caseId: string) => request<Hearing[]>(`/cases/${caseId}/hearings`),
    addHearing: (caseId: string, data: { date: string; time: string; type: string; notes?: string }) =>
      request<Hearing>(`/cases/${caseId}/hearings`, { method: "POST", body: JSON.stringify(data) }),

    getFiles: (caseId: string) => request<CaseFile[]>(`/cases/${caseId}/files`),
    addFile: (caseId: string, data: Omit<CaseFile, "id" | "lawyerId" | "caseId" | "addedAt">) =>
      request<CaseFile>(`/cases/${caseId}/files`, { method: "POST", body: JSON.stringify(data) }),
  },

  hearings: {
    agenda: (startDate?: string, endDate?: string) => {
      const params = new URLSearchParams();
      if (startDate) params.set("startDate", startDate);
      if (endDate) params.set("endDate", endDate);
      const qs = params.toString();
      return request<Hearing[]>(`/hearings/agenda${qs ? `?${qs}` : ""}`);
    },
    delete: (id: string) => request<void>(`/hearings/${id}`, { method: "DELETE" }),
  },

  tasks: {
    list: (q?: string, status?: string, priority?: string, date?: string) => {
      const params = new URLSearchParams();
      if (q) params.set("q", q);
      if (status && status !== "all") params.set("status", status);
      if (priority && priority !== "all") params.set("priority", priority);
      if (date) params.set("date", date);
      const qs = params.toString();
      return request<Task[]>(`/tasks${qs ? `?${qs}` : ""}`);
    },
    get: (id: string) => request<Task>(`/tasks/${id}`),
    create: (data: Omit<Task, "id" | "lawyerId" | "createdAt" | "completed">) =>
      request<Task>("/tasks", { method: "POST", body: JSON.stringify(data) }),
    update: (id: string, data: Partial<Task>) =>
      request<Task>(`/tasks/${id}`, { method: "PUT", body: JSON.stringify(data) }),
    toggle: (id: string) => request<Task>(`/tasks/${id}/toggle`, { method: "PATCH" }),
    delete: (id: string) => request<void>(`/tasks/${id}`, { method: "DELETE" }),
  },

  templates: {
    list: (q?: string, ext?: string) => {
      const params = new URLSearchParams();
      if (q) params.set("q", q);
      if (ext && ext !== "all") params.set("ext", ext);
      const qs = params.toString();
      return request<Template[]>(`/templates${qs ? `?${qs}` : ""}`);
    },
    create: (data: Omit<Template, "id" | "lawyerId" | "addedAt">) =>
      request<Template>("/templates", { method: "POST", body: JSON.stringify(data) }),
    rename: (id: string, name: string) =>
      request<Template>(`/templates/${id}/rename`, { method: "PATCH", body: JSON.stringify({ name }) }),
    delete: (id: string) => request<void>(`/templates/${id}`, { method: "DELETE" }),
  },

  caseFiles: {
    delete: (id: string) => request<void>(`/case-files/${id}`, { method: "DELETE" }),
  },

  transactions: {
    list: (q?: string, type?: string) => {
      const params = new URLSearchParams();
      if (q) params.set("q", q);
      if (type && type !== "all") params.set("type", type);
      const qs = params.toString();
      return request<Tx[]>(`/transactions${qs ? `?${qs}` : ""}`);
    },
    create: (data: Omit<Tx, "id" | "lawyerId" | "createdAt">) =>
      request<Tx>("/transactions", { method: "POST", body: JSON.stringify(data) }),
    update: (id: string, data: Partial<Tx>) =>
      request<Tx>(`/transactions/${id}`, { method: "PUT", body: JSON.stringify(data) }),
    delete: (id: string) => request<void>(`/transactions/${id}`, { method: "DELETE" }),
    summary: () => request<Array<{ clientId: string; caseId?: string; fees: number; expenses: number; net: number }>>("/transactions/summary"),
    totals: () => request<{ totalFees: number; totalExpenses: number; netIncome: number }>("/transactions/totals"),
  },

  dashboard: {
    summary: () => request<unknown>("/dashboard/summary"),
  },

  activities: {
    list: (count = 10) => request<Activity[]>(`/activities?count=${count}`),
  },
};
