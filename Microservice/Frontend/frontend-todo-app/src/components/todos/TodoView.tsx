import React, { useState } from 'react';
import TodoList from './TodoList';
import TodoModal from './TodoModal';
import { todoService} from '../../services/todoService';
import type { Todo } from '../../services/todoService';

interface TodoViewProps {
  groupId?: number;
  title: string;
}

const TodoView: React.FC<TodoViewProps> = ({ groupId, title }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTodo, setEditingTodo] = useState<Todo | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleCreate = async (data: { description: string; dueDate?: string }) => {
    setIsLoading(true);
    try {
      await todoService.createTodo({
        description: data.description,
        dueDate: data.dueDate,
        groupId
      });
      setIsModalOpen(false);
      setRefreshKey(prev => prev + 1);
    } catch (error) {
      console.error('Failed to create todo:', error);
      alert('Failed to create todo');
    } finally {
      setIsLoading(false);
    }
  };

  const handleUpdate = async (data: { description: string; dueDate?: string }) => {
    if (!editingTodo) return;
    setIsLoading(true);
    try {
      await todoService.updateTodo(editingTodo.todoId, {
        description: data.description,
        dueDate: data.dueDate
      });
      setIsModalOpen(false);
      setEditingTodo(null);
      setRefreshKey(prev => prev + 1);
    } catch (error) {
      console.error('Failed to update todo:', error);
      alert('Failed to update todo');
    } finally {
      setIsLoading(false);
    }
  };

  const handleEdit = (todo: Todo) => {
    setEditingTodo(todo);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingTodo(null);
  };

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-gray-900">{title}</h2>
        <button
          onClick={() => setIsModalOpen(true)}
          className="px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg transition-colors"
        >
          + Add Todo
        </button>
      </div>
      <TodoList groupId={groupId} onEdit={handleEdit} refreshTrigger={refreshKey} />
      <TodoModal
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        onSubmit={editingTodo ? handleUpdate : handleCreate}
        isLoading={isLoading}
        editingTodo={editingTodo}
      />
    </div>
  );
};

export default TodoView;
