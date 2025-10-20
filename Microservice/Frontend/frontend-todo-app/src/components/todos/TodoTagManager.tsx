import React, { useState, useEffect } from 'react';
import tagService from '../../services/tagService';
import type { Tag } from '../../services/tagService';

interface TodoTagManagerProps {
  todoId: number;
  currentTags: Array<{ tagId: number; tagName: string; color?: string }>;
  onTagsChange: () => void;
}

const TodoTagManager: React.FC<TodoTagManagerProps> = ({ todoId, currentTags, onTagsChange }) => {
  const [allTags, setAllTags] = useState<Tag[]>([]);
  const [showDropdown, setShowDropdown] = useState(false);

  useEffect(() => {
    loadTags();
  }, []);

  const loadTags = async () => {
    try {
      const tags = await tagService.getTags();
      setAllTags(tags);
    } catch (error) {
      console.error('Failed to load tags:', error);
    }
  };

  const handleAddTag = async (tagId: number) => {
    try {
      await tagService.addTagToTodo(todoId, tagId);
      onTagsChange();
      setShowDropdown(false);
    } catch (error) {
      console.error('Failed to add tag:', error);
    }
  };

  const handleRemoveTag = async (tagId: number) => {
    try {
      await tagService.removeTagFromTodo(todoId, tagId);
      onTagsChange();
    } catch (error) {
      console.error('Failed to remove tag:', error);
    }
  };

  const availableTags = allTags.filter(
    tag => !currentTags.some(ct => ct.tagId === tag.tagId)
  );

  return (
    <div className="flex flex-wrap gap-2 items-center">
      {currentTags.map(tag => (
        <span
          key={tag.tagId}
          className="inline-flex items-center px-2 py-1 rounded text-xs font-medium"
          style={{ backgroundColor: tag.color + '20', color: tag.color }}
        >
          {tag.tagName}
          <button
            onClick={() => handleRemoveTag(tag.tagId)}
            className="ml-1 hover:opacity-70"
          >
            ×
          </button>
        </span>
      ))}
      <div className="relative">
        <button
          onClick={() => setShowDropdown(!showDropdown)}
          className="px-2 py-1 text-xs text-gray-600 hover:text-gray-900 border border-dashed border-gray-300 rounded hover:border-gray-400"
        >
          + Tag
        </button>
        {showDropdown && availableTags.length > 0 && (
          <div className="absolute z-10 mt-1 bg-white border rounded shadow-lg max-h-48 overflow-y-auto">
            {availableTags.map(tag => (
              <button
                key={tag.tagId}
                onClick={() => handleAddTag(tag.tagId)}
                className="block w-full text-left px-3 py-2 text-sm hover:bg-gray-100"
              >
                <span
                  className="inline-block w-3 h-3 rounded-full mr-2"
                  style={{ backgroundColor: tag.color }}
                />
                {tag.tagName}
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default TodoTagManager;
