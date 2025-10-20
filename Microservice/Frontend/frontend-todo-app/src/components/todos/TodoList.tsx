import React, { useState, useEffect } from 'react';
import { todoService} from '../../services/todoService';
import type { Todo } from '../../services/todoService';
import TodoTagManager from './TodoTagManager';

interface TodoListProps {
  groupId?: number;
  onEdit: (todo: Todo) => void;
  refreshTrigger?: number;
}

const TodoList: React.FC<TodoListProps> = ({ groupId, onEdit, refreshTrigger }) => {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadTodos();
  }, [groupId, refreshTrigger]);

  const loadTodos = async () => {
    try {
      setLoading(true);
      const data = await todoService.getTodos(groupId);
      setTodos(data);
    } catch (error) {
      console.error('Failed to load todos:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleToggle = async (todo: Todo) => {
    try {
      await todoService.updateTodo(todo.todoId, { isDone: !todo.isDone });
      loadTodos();
    } catch (error) {
      console.error('Failed to update todo:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this todo?')) return;
    try {
      await todoService.deleteTodo(id);
      loadTodos();
    } catch (error) {
      console.error('Failed to delete todo:', error);
    }
  };

  if (loading) {
    return <div className="text-center py-8 text-gray-500">Loading...</div>;
  }

  if (todos.length === 0) {
    return <div className="text-center py-8 text-gray-500">No todos yet. Create one!</div>;
  }

  return (
    <div className="space-y-2">
      {todos.map((todo) => (
        <div
          key={todo.todoId}
          className="flex items-center justify-between p-4 bg-white rounded-lg border hover:shadow-md transition-shadow"
        >
          <div className="flex items-center space-x-3 flex-1">
            <input
              type="checkbox"
              checked={todo.isDone || false}
              onChange={() => handleToggle(todo)}
              className="w-5 h-5 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
            />
            <div className="flex-1">
              <p className={`text-gray-900 ${todo.isDone ? 'line-through text-gray-500' : ''}`}>
                {todo.description}
              </p>
              <div className="mt-2">
                <TodoTagManager
                  todoId={todo.todoId}
                  currentTags={todo.tags}
                  onTagsChange={loadTodos}
                />
              </div>
              {todo.dueDate && (
                <p className="text-xs text-gray-500 mt-1">
                  Due: {new Date(todo.dueDate).toLocaleDateString()}
                </p>
              )}
            </div>
          </div>
          <div className="flex space-x-2">
            <button
              onClick={() => onEdit(todo)}
              className="p-2 text-blue-600 hover:bg-blue-50 rounded transition-colors"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
              </svg>
            </button>
            <button
              onClick={() => handleDelete(todo.todoId)}
              className="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};

export default TodoList;
