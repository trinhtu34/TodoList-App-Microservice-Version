import React, { useState } from 'react';

interface SidebarProps {
  activeView: string;
  onViewChange: (view: string) => void;
  onAddTask: () => void;
  onSearch?: (query: string) => void;
}

const Sidebar: React.FC<SidebarProps> = ({ activeView, onViewChange, onAddTask, onSearch }) => {
  const [searchQuery, setSearchQuery] = useState('');

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
        <button className="w-full flex items-center space-x-3 px-3 py-2.5 text-gray-700 hover:bg-gray-50 rounded-lg text-sm transition-all duration-200">
          <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          <span className="font-normal">Add a team</span>
        </button>
        <button className="w-full flex items-center space-x-3 px-3 py-2.5 text-gray-700 hover:bg-gray-50 rounded-lg text-sm transition-all duration-200">
          <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span className="font-normal">Help & resources</span>
        </button>
      </div>
    </div>
  );
};

export default Sidebar;
