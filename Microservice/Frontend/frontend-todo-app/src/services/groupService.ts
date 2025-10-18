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

export interface Invitation {
  invitationId: number;
  groupId: number;
  groupName: string;
  invitedBy: string;
  invitedUser: string;
  status: string;
  createdAt: string;
  expiresAt: string | null;
}

export interface CreateInvitationRequest {
  groupId: number;
  invitedUser: string;
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

  getGroupById: async (groupId: number): Promise<Group> => {
    const response = await groupApi.get(`/group/${groupId}`);
    return response.data;
  },

  // Invitation APIs
  createInvitation: async (data: CreateInvitationRequest): Promise<Invitation> => {
    const response = await groupApi.post('/invitation', data);
    return response.data;
  },

  getUserInvitations: async (): Promise<Invitation[]> => {
    const response = await groupApi.get('/invitation');
    return response.data;
  },

  acceptInvitation: async (invitationId: number): Promise<void> => {
    await groupApi.post(`/invitation/${invitationId}/accept`);
  },

  declineInvitation: async (invitationId: number): Promise<void> => {
    await groupApi.post(`/invitation/${invitationId}/decline`);
  },

  getUserByEmail: async (email: string): Promise<string> => {
    const response = await groupApi.get(`/user/by-email/${encodeURIComponent(email)}`);
    return response.data.cognitoSub;
  },
};

export interface Member {
  userId: string;
  role: string;
  nickname: string | null;
  joinedAt: string;
  isMuted: boolean;
  isActive: boolean;
}

export const memberService = {
  getGroupMembers: async (groupId: number): Promise<Member[]> => {
    const response = await groupApi.get(`/groups/${groupId}/member`);
    return response.data;
  },
};
