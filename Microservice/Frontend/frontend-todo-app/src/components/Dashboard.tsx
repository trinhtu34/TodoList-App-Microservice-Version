import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import Sidebar from './Sidebar';
import TagManager from './TagManager';
import InvitationList from './InvitationList';
import InviteUserModal from './InviteUserModal';
import GroupView from './GroupView';
import { groupService } from '../services/groupService';
import type { Group } from '../services/groupService';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [activeView, setActiveView] = useState('inbox');
  const [groups, setGroups] = useState<Group[]>([]);
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [invitingGroup, setInvitingGroup] = useState<Group | null>(null);
  const [isInviting, setIsInviting] = useState(false);

  useEffect(() => {
    loadGroups();
  }, []);

  const loadGroups = async () => {
    try {
      const data = await groupService.getUserGroups();
      setGroups(data);
    } catch (error) {
      console.error('Failed to load groups:', error);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleAddTask = () => {
    console.log('Add task clicked');
  };

  const handleSearch = (query: string) => {
    console.log('Search:', query);
  };

  const handleInviteUser = (group: Group) => {
    setInvitingGroup(group);
    setShowInviteModal(true);
  };

  const handleSendInvitation = async (email: string) => {
    if (!invitingGroup) return;
    setIsInviting(true);
    try {
      // Convert email to cognito_sub
      const cognitoSub = await groupService.getUserByEmail(email);
      
      await groupService.createInvitation({
        groupId: invitingGroup.groupId,
        invitedUser: cognitoSub
      });
      setShowInviteModal(false);
      setInvitingGroup(null);
      alert('Invitation sent successfully!');
    } catch (error) {
      console.error('Failed to send invitation:', error);
      alert('Failed to send invitation. User not found or error occurred.');
    } finally {
      setIsInviting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex">
      {/* Sidebar */}
      <Sidebar
        activeView={activeView}
        onViewChange={setActiveView}
        onAddTask={handleAddTask}
        onSearch={handleSearch}
        onGroupsChange={setGroups}
      />

      {/* Main Content */}
      <div className="flex-1 flex flex-col">
        {/* Header */}
        <nav className="bg-white shadow-sm">
          <div className="px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between h-16">
              <div className="flex items-center">
                <h1 className="text-xl font-bold text-gray-900">Todo App</h1>
              </div>
              <div className="flex items-center space-x-4">
                <span className="text-gray-700">Xin chào, {user?.name}</span>
                <button
                  onClick={handleLogout}
                  className="px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-md hover:bg-indigo-700"
                >
                  Đăng xuất
                </button>
              </div>
            </div>
          </div>
        </nav>

        {/* Main Content Area */}
        <main className="flex-1 p-6">
          {activeView === 'filters' ? (
            <TagManager isPremium={user?.groups?.includes('Premium-user') || false} />
          ) : activeView === 'invitations' ? (
            <div className="bg-white rounded-lg shadow p-6">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">Group Invitations</h2>
              <InvitationList />
            </div>
          ) : activeView.startsWith('group-') ? (
            <GroupView
              groupId={parseInt(activeView.replace('group-', ''))}
              onInviteClick={() => {
                const group = groups.find(g => `group-${g.groupId}` === activeView);
                if (group) handleInviteUser(group);
              }}
            />
          ) : (
            <div className="bg-white rounded-lg shadow p-6">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                {activeView.charAt(0).toUpperCase() + activeView.slice(1)}
              </h2>
              <p className="text-gray-600">Chào mừng bạn đến với Todo App!</p>
            </div>
          )}
        </main>
      </div>

      <InviteUserModal
        isOpen={showInviteModal}
        onClose={() => {
          setShowInviteModal(false);
          setInvitingGroup(null);
        }}
        onSubmit={handleSendInvitation}
        isLoading={isInviting}
        groupName={invitingGroup?.groupName || ''}
      />
    </div>
  );
};

export default Dashboard;
