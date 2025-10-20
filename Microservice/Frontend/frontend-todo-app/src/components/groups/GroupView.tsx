import React, { useState, useEffect } from 'react';
import { groupService, memberService } from '../../services/groupService';
import type { Group, Member } from '../../services/groupService';
import { TodoView } from '../todos';
import { useAuth } from '../../context/AuthContext';

interface GroupViewProps {
  groupId: number;
  onInviteClick: () => void;
}

const GroupView: React.FC<GroupViewProps> = ({ groupId, onInviteClick }) => {
  const { user } = useAuth();
  const [group, setGroup] = useState<Group | null>(null);
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'chat' | 'todos' | 'about'>('chat');
  const [editingNickname, setEditingNickname] = useState(false);
  const [newNickname, setNewNickname] = useState('');

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

  const handleUpdateNickname = async () => {
    try {
      await memberService.updateMemberSettings(groupId, newNickname);
      setEditingNickname(false);
      loadGroupData();
    } catch (error) {
      console.error('Failed to update nickname:', error);
      alert('Failed to update nickname');
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
      {/* Main Content */}
      <div className="flex-1 flex flex-col bg-white">
        {/* Header */}
        <div className="border-b">
          <div className="p-4 flex justify-between items-center">
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

          {/* Tabs */}
          <div className="flex border-t">
            <button
              onClick={() => setActiveTab('chat')}
              className={`px-6 py-3 font-medium text-sm transition-colors ${activeTab === 'chat'
                  ? 'text-blue-600 border-b-2 border-blue-600'
                  : 'text-gray-600 hover:text-gray-900'
                }`}
            >
              Chat
            </button>
            <button
              onClick={() => setActiveTab('todos')}
              className={`px-6 py-3 font-medium text-sm transition-colors ${activeTab === 'todos'
                  ? 'text-blue-600 border-b-2 border-blue-600'
                  : 'text-gray-600 hover:text-gray-900'
                }`}
            >
              Todos
            </button>
            <button
              onClick={() => setActiveTab('about')}
              className={`px-6 py-3 font-medium text-sm transition-colors ${activeTab === 'about'
                  ? 'text-blue-600 border-b-2 border-blue-600'
                  : 'text-gray-600 hover:text-gray-900'
                }`}
            >
              About
            </button>
          </div>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto">
          {activeTab === 'chat' ? (
            <div className="flex flex-col h-full">
              <div className="flex-1 p-6 bg-gray-50">
                <div className="text-center text-gray-500 mt-8">
                  <p>Chat feature coming soon...</p>
                  <p className="text-sm mt-2">This is where messages will appear</p>
                </div>
              </div>
              <div className="p-4 bg-white border-t">
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
          ) : activeTab === 'todos' ? (
            <div className="p-6">
              <TodoView
                title=""
                groupId={groupId}
                members={members.map(member => ({
                  userId: member.userId,
                  nickname: member.nickname || undefined,
                  role: member.role
                }))}
              />
            </div>
          ) : (
            <div className="p-6 max-w-3xl">
              {/* Group Description */}
              <div className="mb-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Description</h3>
                <p className="text-gray-600">
                  {group.groupDescription || 'No description'}
                </p>
                <div className="mt-3 text-sm text-gray-500">
                  Created {new Date(group.createdAt || '').toLocaleDateString()}
                </div>
              </div>

              {/* Members List */}
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">
                  Members ({members.length})
                </h3>
                <div className="space-y-2">
                  {members.map((member) => {
                    const isCurrentUser = member.userId === user?.sub;

                    return (
                      <div
                        key={member.userId}
                        className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                      >
                        <div className="flex items-center space-x-3 flex-1">
                          <div className="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center text-white font-medium">
                            {member.userId.substring(0, 2).toUpperCase()}
                          </div>
                          <div className="flex-1">
                            <div className="text-sm font-medium text-gray-900">
                              {member.nickname || member.userId.substring(0, 8)}
                              {isCurrentUser && ' (You)'}
                            </div>
                            <div className="text-xs text-gray-500 capitalize">
                              {member.role}
                            </div>
                          </div>
                        </div>
                        <div className="flex items-center space-x-2">
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
                          {isCurrentUser && (
                            <button
                              onClick={() => {
                                setNewNickname(member.nickname || '');
                                setEditingNickname(true);
                              }}
                              className="p-1 text-gray-500 hover:text-blue-600 rounded"
                              title="Edit nickname"
                            >
                              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                              </svg>
                            </button>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Edit Nickname Modal */}
      {editingNickname && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Edit Nickname</h2>
            <input
              type="text"
              value={newNickname}
              onChange={(e) => setNewNickname(e.target.value)}
              placeholder="Enter nickname"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none mb-4"
            />
            <div className="flex justify-end space-x-2">
              <button
                onClick={() => setEditingNickname(false)}
                className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleUpdateNickname}
                className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default GroupView;
