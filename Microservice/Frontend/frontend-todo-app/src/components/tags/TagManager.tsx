import React, { useState, useEffect } from 'react';
import tagService from '../../services/tagService';
import type { Tag } from '../../services/tagService';

interface TagManagerProps {
  isPremium: boolean;
}

const TagManager: React.FC<TagManagerProps> = ({ isPremium }) => {
  const [tags, setTags] = useState<Tag[]>([]);
  const [newTagName, setNewTagName] = useState('');
  const [newTagColor, setNewTagColor] = useState('#808080');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isPremium) {
      loadTags();
    }
  }, [isPremium]);

  const loadTags = async () => {
    try {
      const data = await tagService.getTags();
      setTags(data);
    } catch (error: any) {
      console.error('Error loading tags:', error);
      setError('Không thể tải tags');
    }
  };

  const handleCreateTag = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!newTagName.trim()) {
      return;
    }

    try {
      setLoading(true);
      setError('');
      await tagService.createTag(newTagName.trim(), newTagColor);
      setNewTagName('');
      setNewTagColor('#808080');
      await loadTags();
    } catch (error: any) {
      setError(error.response?.data?.message || 'Lỗi tạo tag');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteTag = async (tagId: number) => {
    if (!confirm('Bạn có chắc muốn xóa tag này?')) {
      return;
    }

    try {
      await tagService.deleteTag(tagId);
      await loadTags();
    } catch (error: any) {
      setError('Lỗi xóa tag');
    }
  };

  if (!isPremium) {
    return (
      <div className="bg-white rounded-lg shadow p-6">
        <h3 className="text-xl font-bold text-gray-900 mb-4">Tags (Premium Feature)</h3>
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
          <p className="text-yellow-800">Nâng cấp lên Premium để sử dụng tính năng Tags!</p>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow p-6">
      <h3 className="text-xl font-bold text-gray-900 mb-4">Quản lý Tags</h3>
      
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      <form onSubmit={handleCreateTag} className="mb-6">
        <div className="flex gap-2">
          <input
            type="text"
            value={newTagName}
            onChange={(e) => setNewTagName(e.target.value)}
            placeholder="Tên tag..."
            maxLength={50}
            className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
          <input
            type="color"
            value={newTagColor}
            onChange={(e) => setNewTagColor(e.target.value)}
            className="w-12 h-10 border border-gray-300 rounded-lg cursor-pointer"
          />
          <button
            type="submit"
            disabled={loading}
            className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50"
          >
            {loading ? 'Đang tạo...' : 'Thêm'}
          </button>
        </div>
      </form>

      <div className="space-y-2">
        {tags.length === 0 ? (
          <p className="text-gray-500 text-center py-4">Chưa có tag nào</p>
        ) : (
          tags.map(tag => (
            <div
              key={tag.tagId}
              className="flex items-center justify-between p-3 border border-gray-200 rounded-lg hover:bg-gray-50"
            >
              <div className="flex items-center gap-3">
                <div
                  className="w-4 h-4 rounded-full"
                  style={{ backgroundColor: tag.color || '#808080' }}
                />
                <span className="font-medium text-gray-900">{tag.tagName}</span>
              </div>
              <button
                onClick={() => handleDeleteTag(tag.tagId)}
                className="text-red-500 hover:text-red-700 font-bold text-xl"
              >
                ×
              </button>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default TagManager;
