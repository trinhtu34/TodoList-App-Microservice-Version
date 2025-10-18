import React, { useState, useEffect } from 'react';
import { groupService, memberService } from '../services/groupService';
import type { Group, Member } from '../services/groupService';

interface GroupViewProps {
  groupId: number;
  onInviteClick: () => void;
}

const GroupView: React.FC<GroupViewProps> = ({ groupId, onInviteClick }) => {
  const [group, setGroup] = useState<Group | null>(null);
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadGroupData();
  }, [groupId]);

  const loadGroupData = async () => {
    try {
      setLoading(true);
      const [groupData, membersData] = await Promise.all([
        groupService.getGroupById(groupId),
        memberService.getGroupMembers(groupId)
      ]);
      setGroup(groupData);
      setMembers(membersData);
    } catch (error) {
      console.error('Failed to load group data:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-full">
        <div className="text-gray-500">Loading...</div>
      </div>
    );
  }

  if (!group) {
    return (
      <div className="flex justify-center items-center h-full">
        <div className="text-gray-500">Group not found</div>
      </div>
    );
  }

  return (
    <div className="h-full flex">
      {/* Main Content - Chat Area */}
      <div className="flex-1 flex flex-col">
        <div className="bg-white border-b p-4">
          <div className="flex justify-between items-center">
            <div>
              <h2 className="text-xl font-bold text-gray-900">{group.groupName}</h2>
              <p className="text-sm text-gray-500">{members.length} members</p>
            </div>
            <button
              onClick={onInviteClick}
              className="px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg transition-colors"
            >
              Invite User
            </button>
          </div>
        </div>

        {/* Chat Area - Placeholder */}
        <div className="flex-1 bg-gray-50 p-4 overflow-y-auto">
          <div className="text-center text-gray-500 mt-8">
            <p>Chat feature coming soon...</p>
            <p className="text-sm mt-2">This is where messages will appear</p>
          </div>
        </div>

        {/* Message Input - Placeholder */}
        <div className="bg-white border-t p-4">
          <div className="flex space-x-2">
            <input
              type="text"
              placeholder="Type a message..."
              className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none"
              disabled
            />
            <button
              className="px-6 py-2 bg-blue-500 text-white rounded-lg opacity-50 cursor-not-allowed"
              disabled
            >
              Send
            </button>
          </div>
        </div>
      </div>

      {/* Right Sidebar - Group Info */}
      <div className="w-80 bg-white border-l overflow-y-auto">
        {/* Group Description */}
        <div className="p-4 border-b">
          <h3 className="font-semibold text-gray-900 mb-2">About</h3>
          <p className="text-sm text-gray-600">
            {group.groupDescription || 'No description'}
          </p>
          <div className="mt-3 text-xs text-gray-500">
            Created {new Date(group.createdAt || '').toLocaleDateString()}
          </div>
        </div>

        {/* Members List */}
        <div className="p-4">
          <h3 className="font-semibold text-gray-900 mb-3">
            Members ({members.length})
          </h3>
          <div className="space-y-2">
            {members.map((member) => (
              <div
                key={member.userId}
                className="flex items-center justify-between p-2 hover:bg-gray-50 rounded"
              >
                <div className="flex items-center space-x-3">
                  <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center text-white text-sm font-medium">
                    {member.userId.substring(0, 2).toUpperCase()}
                  </div>
                  <div>
                    <div className="text-sm font-medium text-gray-900">
                      {member.nickname || member.userId.substring(0, 8)}
                    </div>
                    <div className="text-xs text-gray-500 capitalize">
                      {member.role}
                    </div>
                  </div>
                </div>
                {member.role === 'owner' && (
                  <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-1 rounded">
                    Owner
                  </span>
                )}
                {member.role === 'admin' && (
                  <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                    Admin
                  </span>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default GroupView;
