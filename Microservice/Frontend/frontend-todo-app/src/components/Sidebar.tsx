import React, { useState, useEffect } from 'react';
import { groupService } from '../services/groupService';
import type { Group } from '../services/groupService';
import CreateGroupModal from './CreateGroupModal';
import EditGroupModal from './EditGroupModal';

interface SidebarProps {
  activeView: string;
  onViewChange: (view: string) => void;
  onAddTask: () => void;
  onSearch?: (query: string) => void;
  onGroupsChange?: (groups: Group[]) => void;
}

const Sidebar: React.FC<SidebarProps> = ({ activeView, onViewChange, onAddTask, onSearch, onGroupsChange }) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [groups, setGroups] = useState<Group[]>([]);
  const [isAddingGroup, setIsAddingGroup] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<Group | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [invitationCount, setInvitationCount] = useState(0);

  useEffect(() => {
    loadGroups();
    loadInvitationCount();
  }, []);

  const loadGroups = async () => {
    try {
      const data = await groupService.getUserGroups();
      setGroups(data);
      if (onGroupsChange) onGroupsChange(data);
    } catch (error) {
      console.error('Failed to load groups:', error);
    }
  };

  const loadInvitationCount = async () => {
    try {
      const invitations = await groupService.getUserInvitations();
      setInvitationCount(invitations.length);
    } catch (error) {
      console.error('Failed to load invitations:', error);
    }
  };

  const handleAddGroup = async (groupName: string, groupDescription: string) => {
    setIsAddingGroup(true);
    try {
      const newGroup = await groupService.createGroup({
        groupName,
        groupDescription: groupDescription || null,
      });
      const updatedGroups = [...groups, newGroup];
      setGroups(updatedGroups);
      if (onGroupsChange) onGroupsChange(updatedGroups);
      setIsModalOpen(false);
    } catch (error) {
      console.error('Failed to create group:', error);
    } finally {
      setIsAddingGroup(false);
    }
  };

  const handleEditGroup = async (groupName: string, groupDescription: string) => {
    if (!editingGroup) return;
    setIsAddingGroup(true);
    try {
      const updatedGroup = await groupService.updateGroup(editingGroup.groupId, {
        groupName,
        groupDescription: groupDescription || null,
      });
      const updatedGroups = groups.map(g => g.groupId === updatedGroup.groupId ? updatedGroup : g);
      setGroups(updatedGroups);
      if (onGroupsChange) onGroupsChange(updatedGroups);
      setIsEditModalOpen(false);
      setEditingGroup(null);
    } catch (error) {
      console.error('Failed to update group:', error);
    } finally {
      setIsAddingGroup(false);
    }
  };

  const handleDeleteGroup = async (group: Group) => {
    if (!window.confirm(`Are you sure you want to delete "${group.groupName}"?`)) return;
    try {
      await groupService.deleteGroup(group.groupId);
      const updatedGroups = groups.filter(g => g.groupId !== group.groupId);
      setGroups(updatedGroups);
      if (onGroupsChange) onGroupsChange(updatedGroups);
      if (activeView === `group-${group.groupId}`) {
        onViewChange('inbox');
      }
    } catch (error) {
      console.error('Failed to delete group:', error);
      alert('Failed to delete group. You must be the owner.');
    }
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (onSearch) {
      onSearch(searchQuery);
    }
  };

  const menuItems = [
    {
      id: 'inbox',
      label: 'Inbox',
      count: 3,
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
      )
    },
    {
      id: 'today',
      label: 'Today',
      count: 1,
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      )
    },
    {
      id: 'upcoming',
      label: 'Upcoming',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      )
    },
    {
      id: 'filters',
      label: 'Filters & Labels',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
        </svg>
      )
    },
    {
      id: 'completed',
      label: 'Completed',
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      )
    },
    {
      id: 'invitations',
      label: 'Invitations',
      count: invitationCount,
      icon: (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
        </svg>
      )
    }
  ];

  const projects = [
    { id: 'home', label: 'Home', emoji: '🏡', count: 5 },
    { id: 'education', label: 'Education', emoji: '📚', count: 4 },
    { id: 'work', label: 'My work', emoji: '🎯', count: 6 }
  ];

  return (
    <div className="w-64 bg-white border-r border-gray-200 h-full flex flex-col">
      {/* Add Task Button */}
      <div className="p-4">
        <button
          onClick={onAddTask}
          className="w-full bg-blue-500 hover:bg-blue-600 text-white font-medium py-2.5 px-4 rounded-lg text-sm transition-colors duration-200 shadow-sm hover:shadow"
        >
          + Add Task
        </button>
      </div>

      {/* Search */}
      <div className="px-4 pb-4">
        <div className="relative">
          <svg className="w-4 h-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            type="text"
            placeholder="Search tasks..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onKeyPress={(e) => {
              if (e.key === 'Enter') {
                handleSearch(e);
              }
            }}
            className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all"
          />
        </div>
      </div>

      {/* Navigation Menu */}
      <nav className="flex-1 px-4 overflow-y-auto">
        <ul className="space-y-1.5">
          {menuItems.map((item) => (
            <li key={item.id}>
              <button
                onClick={() => onViewChange(item.id)}
                className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-sm transition-all duration-200 ${
                  activeView === item.id
                    ? 'bg-blue-50 text-blue-600 font-medium shadow-sm'
                    : 'text-gray-700 hover:bg-gray-50 font-normal'
                }`}
              >
                <div className="flex items-center space-x-3">
                  <div className={activeView === item.id ? 'text-blue-600' : 'text-gray-500'}>
                    {item.icon}
                  </div>
                  <span>{item.label}</span>
                </div>
                {item.count && (
                  <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${
                    activeView === item.id 
                      ? 'bg-blue-100 text-blue-600' 
                      : 'bg-gray-100 text-gray-600'
                  }`}>
                    {item.count}
                  </span>
                )}
              </button>
            </li>
          ))}
        </ul>

        {/* My Projects Section */}
        <div className="mt-8">
          <h3 className="text-xs font-semibold text-gray-400 uppercase px-3 mb-3 tracking-wider">
            My Projects
          </h3>
          <ul className="space-y-1.5">
            {projects.map((project) => (
              <li key={project.id}>
                <button
                  onClick={() => onViewChange(project.id)}
                  className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-sm transition-all duration-200 ${
                    activeView === project.id
                      ? 'bg-blue-50 text-blue-600 font-medium shadow-sm'
                      : 'text-gray-700 hover:bg-gray-50 font-normal'
                  }`}
                >
                  <div className="flex items-center space-x-3">
                    <span className="text-base">{project.emoji}</span>
                    <span>{project.label}</span>
                  </div>
                  <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${
                    activeView === project.id 
                      ? 'bg-blue-100 text-blue-600' 
                      : 'bg-gray-100 text-gray-600'
                  }`}>
                    {project.count}
                  </span>
                </button>
              </li>
            ))}
          </ul>
        </div>
      </nav>

      {/* Footer Actions */}
      <div className="p-4 border-t border-gray-200 space-y-1.5">
        <button 
          onClick={() => setIsModalOpen(true)}
          className="w-full flex items-center space-x-3 px-3 py-2.5 text-gray-700 hover:bg-gray-50 rounded-lg text-sm transition-all duration-200"
        >
          <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          <span className="font-normal">Add a group</span>
        </button>
        {groups.map((group) => (
          <div key={group.groupId} className="flex items-center space-x-1">
            <button
              onClick={() => onViewChange(`group-${group.groupId}`)}
              className={`flex-1 flex items-center space-x-3 px-3 py-2.5 rounded-lg text-sm transition-all duration-200 ${
                activeView === `group-${group.groupId}`
                  ? 'bg-blue-50 text-blue-600 font-medium'
                  : 'text-gray-700 hover:bg-gray-50'
              }`}
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <span className="font-normal truncate">{group.groupName || 'Unnamed Group'}</span>
            </button>
            <button
              onClick={() => {
                setEditingGroup(group);
                setIsEditModalOpen(true);
              }}
              className="p-2 text-gray-500 hover:text-blue-600 hover:bg-gray-100 rounded transition-colors"
              title="Edit group"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
              </svg>
            </button>
            <button
              onClick={() => handleDeleteGroup(group)}
              className="p-2 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
              title="Delete group"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        ))}
        <button className="w-full flex items-center space-x-3 px-3 py-2.5 text-gray-700 hover:bg-gray-50 rounded-lg text-sm transition-all duration-200">
          <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span className="font-normal">Help & resources</span>
        </button>
      </div>

      <CreateGroupModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleAddGroup}
        isLoading={isAddingGroup}
      />

      <EditGroupModal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setEditingGroup(null);
        }}
        onSubmit={handleEditGroup}
        isLoading={isAddingGroup}
        initialName={editingGroup?.groupName || ''}
        initialDescription={editingGroup?.groupDescription || ''}
      />
    </div>
  );
};

export default Sidebar;
