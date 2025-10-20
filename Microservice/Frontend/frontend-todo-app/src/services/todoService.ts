import todoApi from '../config/todoApi';

export interface Todo {
  todoId: number;
  description: string;
  isDone: boolean;
  dueDate?: string;
  createAt?: string;
  updateAt?: string;
  groupId?: number;
  assignedTo?: string;
  tags: Array<{
    tagId: number;
    tagName: string;
    color?: string;
  }>;
}

export interface CreateTodoRequest {
  description: string;
  dueDate?: string;
  groupId?: number;
  assignedTo?: string;
  tagIds?: number[];
}

export interface UpdateTodoRequest {
  description?: string;
  isDone?: boolean;
  dueDate?: string;
  assignedTo?: string;
  tagIds?: number[];
}

export const todoService = {
  getTodos: async (groupId?: number): Promise<Todo[]> => {
    const params = groupId ? { groupId } : {};
    const response = await todoApi.get('/todos', { params });
    return response.data;
  },

  getTodoById: async (id: number): Promise<Todo> => {
    const response = await todoApi.get(`/todos/${id}`);
    return response.data;
  },

  createTodo: async (data: CreateTodoRequest): Promise<Todo> => {
    const response = await todoApi.post('/todos', data);
    return response.data;
  },

  updateTodo: async (id: number, data: UpdateTodoRequest): Promise<Todo> => {
    const response = await todoApi.put(`/todos/${id}`, data);
    return response.data;
  },

  deleteTodo: async (id: number): Promise<void> => {
    await todoApi.delete(`/todos/${id}`);
  }
};
