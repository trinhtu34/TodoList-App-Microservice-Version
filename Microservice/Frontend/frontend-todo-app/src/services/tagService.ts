import { tagApi } from '../config/api';

interface Tag {
  tagId: number;
  tagName: string;
  color?: string;
  groupId?: number | null;
  createdAt?: string;
}

class TagService {
  async getTags(): Promise<Tag[]> {
    const response = await tagApi.get('/tag');
    return response.data;
  }

  async createTag(tagName: string, color?: string): Promise<Tag> {
    const response = await tagApi.post('/tag', { 
      tagName, 
      color: color || '#808080' 
    });
    return response.data;
  }

  async updateTag(tagId: number, tagName?: string, color?: string): Promise<void> {
    await tagApi.put(`/tag/${tagId}`, { tagName, color });
  }

  async deleteTag(tagId: number): Promise<void> {
    await tagApi.delete(`/tag/${tagId}`);
  }

  async getTagsForTodo(todoId: number): Promise<Tag[]> {
    const response = await tagApi.get(`/tag/todo/${todoId}`);
    return response.data;
  }

  async addTagToTodo(todoId: number, tagId: number): Promise<void> {
    await tagApi.post(`/tag/todo/${todoId}`, { tagId });
  }

  async removeTagFromTodo(todoId: number, tagId: number): Promise<void> {
    await tagApi.delete(`/tag/todo/${todoId}/tag/${tagId}`);
  }
}

const tagServiceInstance = new TagService();
export default tagServiceInstance;
export type { Tag };
