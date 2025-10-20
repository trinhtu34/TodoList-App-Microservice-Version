import { useState, useEffect, useCallback } from 'react';
import { todoService } from '../services/todoService';
import type { Todo, CreateTodoRequest, UpdateTodoRequest } from '../services/todoService';

// Simple cache implementation
const todoCache = new Map<string, { data: Todo[]; timestamp: number }>();
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

// Custom hook for todos with simple caching
export const useTodos = (groupId?: number) => {
  const [data, setData] = useState<Todo[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const cacheKey = `todos-${groupId || 'undefined'}`;

  const fetchTodos = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      // Check cache first
      const cached = todoCache.get(cacheKey);
      if (cached && Date.now() - cached.timestamp < CACHE_DURATION) {
        setData(cached.data);
        setIsLoading(false);
        return;
      }

      // Fetch from API
      const todos = await todoService.getTodos(groupId);
      
      // Update cache
      todoCache.set(cacheKey, { data: todos, timestamp: Date.now() });
      setData(todos);
    } catch (err) {
      setError(err instanceof Error ? err : new Error('Failed to fetch todos'));
    } finally {
      setIsLoading(false);
    }
  }, [groupId, cacheKey]);

  useEffect(() => {
    fetchTodos();
  }, [fetchTodos]);

  return { data, isLoading, error, refetch: fetchTodos };
};

export const useCreateTodo = () => {
  const [isLoading, setIsLoading] = useState(false);

  const mutate = async (data: CreateTodoRequest) => {
    setIsLoading(true);
    try {
      const result = await todoService.createTodo(data);
      
      // Invalidate cache
      todoCache.delete(`todos-${data.groupId || 'undefined'}`);
      todoCache.delete('todos-undefined');
      
      return result;
    } finally {
      setIsLoading(false);
    }
  };

  return { mutate, isLoading };
};

export const useUpdateTodo = () => {
  const [isLoading, setIsLoading] = useState(false);

  const mutate = async ({ id, data }: { id: number; data: UpdateTodoRequest }) => {
    setIsLoading(true);
    try {
      const result = await todoService.updateTodo(id, data);
      
      // Invalidate all todo caches
      todoCache.clear();
      
      return result;
    } finally {
      setIsLoading(false);
    }
  };

  return { mutate, isLoading };
};

export const useDeleteTodo = () => {
  const [isLoading, setIsLoading] = useState(false);

  const mutate = async (id: number) => {
    setIsLoading(true);
    try {
      await todoService.deleteTodo(id);
      
      // Invalidate all todo caches
      todoCache.clear();
    } finally {
      setIsLoading(false);
    }
  };

  return { mutate, isLoading };
};