import React, { memo, useCallback } from 'react';
import { useTodos, useUpdateTodo, useDeleteTodo } from '../../hooks/useTodos';
import type { Todo } from '../../services/todoService';
import TodoTagManager from './TodoTagManager';
import type { Member } from './TodoView';

interface TodoListProps {
  groupId?: number;
  onEdit: (todo: Todo) => void;
  members?: Member[];
}

// Memoized TodoItem component to prevent unnecessary re-renders
const TodoItem = memo(({ 
  todo, 
  onEdit, 
  onToggle, 
  onDelete, 
  members, 
  groupId 
}: {
  todo: Todo;
  onEdit: (todo: Todo) => void;
  onToggle: (todo: Todo) => void;
  onDelete: (id: number) => void;
  members?: Member[];
  groupId?: number;
}) => {
  const handleToggle = useCallback(() => onToggle(todo), [todo, onToggle]);
  const handleEdit = useCallback(() => onEdit(todo), [todo, onEdit]);
  const handleDelete = useCallback(() => onDelete(todo.todoId), [todo.todoId, onDelete]);

  return (
    <div className="flex items-center justify-between p-4 bg-white rounded-lg border hover:shadow-md transition-shadow">
      <div className="flex items-center space-x-3 flex-1">
        <input
          type="checkbox"
          checked={todo.isDone || false}
          onChange={handleToggle}
          className="w-5 h-5 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
        />
        <div className="flex-1">
          <p className={`text-gray-900 ${todo.isDone ? 'line-through text-gray-500' : ''}`}>
            {todo.description}
          </p>
          {groupId && members && todo.cognitoSub && (
            <p className="text-xs text-gray-600 mt-1">
              Created by: {members.find(m => m.userId === todo.cognitoSub)?.nickname || todo.cognitoSub.substring(0, 8)}
            </p>
          )}
          <div className="mt-2">
            <TodoTagManager
              todoId={todo.todoId}
              currentTags={todo.tags || []}
              onTagsChange={() => {}} // Will be handled by React Query
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
          onClick={handleEdit}
          className="p-2 text-blue-600 hover:bg-blue-50 rounded transition-colors"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
          </svg>
        </button>
        <button
          onClick={handleDelete}
          className="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    </div>
  );
});

const TodoListOptimized: React.FC<TodoListProps> = ({ groupId, onEdit, members }) => {
  const { data: todos = [], isLoading, error, refetch } = useTodos(groupId);
  const updateTodoMutation = useUpdateTodo();
  const deleteTodoMutation = useDeleteTodo();

  const handleToggle = useCallback(async (todo: Todo) => {
    try {
      await updateTodoMutation.mutate({ 
        id: todo.todoId, 
        data: { isDone: !todo.isDone } 
      });
      refetch(); // Refetch after update
    } catch (error) {
      console.error('Failed to update todo:', error);
    }
  }, [updateTodoMutation, refetch]);

  const handleDelete = useCallback(async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this todo?')) return;
    try {
      await deleteTodoMutation.mutate(id);
      refetch(); // Refetch after delete
    } catch (error) {
      console.error('Failed to delete todo:', error);
    }
  }, [deleteTodoMutation, refetch]);

  if (isLoading) {
    return (
      <div className="space-y-2">
        {[...Array(3)].map((_, i) => (
          <div key={i} className="animate-pulse p-4 bg-gray-200 rounded-lg h-20" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8 text-red-500">
        Error loading todos. Please try again.
      </div>
    );
  }

  if (todos.length === 0) {
    return <div className="text-center py-8 text-gray-500">No todos yet. Create one!</div>;
  }

  return (
    <div className="space-y-2">
      {todos.map((todo) => (
        <TodoItem
          key={todo.todoId}
          todo={todo}
          onEdit={onEdit}
          onToggle={handleToggle}
          onDelete={handleDelete}
          members={members}
          groupId={groupId}
        />
      ))}
    </div>
  );
};

export default memo(TodoListOptimized);