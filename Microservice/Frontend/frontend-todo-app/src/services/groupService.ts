import groupApi from '../config/groupApi';

export interface Group {
  groupId: number;
  groupName: string | null;
  groupAvatar: string | null;
  groupDescription?: string | null;
  groupType: string;
  createdBy?: string;
  createdAt?: string;
  lastMessageAt: string | null;
  isActive?: boolean;
  memberCount: number;
  userRole: string;
}

export interface CreateGroupRequest {
  groupName: string;
  groupAvatar?: string | null;
  groupDescription?: string | null;
}

export const groupService = {
  createGroup: async (data: CreateGroupRequest): Promise<Group> => {
    const response = await groupApi.post('/group', data);
    return response.data;
  },

  getUserGroups: async (): Promise<Group[]> => {
    const response = await groupApi.get('/group');
    return response.data;
  },

  updateGroup: async (groupId: number, data: CreateGroupRequest): Promise<Group> => {
    const response = await groupApi.put(`/group/${groupId}`, data);
    return response.data;
  },

  deleteGroup: async (groupId: number): Promise<void> => {
    await groupApi.delete(`/group/${groupId}`);
  },
};
