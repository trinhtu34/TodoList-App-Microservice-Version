import React, { useState, useEffect } from 'react';
import { groupService } from '../../services/groupService';
import type { Invitation } from '../../services/groupService';

const InvitationList: React.FC = () => {
  const [invitations, setInvitations] = useState<Invitation[]>([]);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState<number | null>(null);

  useEffect(() => {
    loadInvitations();
  }, []);

  const loadInvitations = async () => {
    try {
      setLoading(true);
      const data = await groupService.getUserInvitations();
      setInvitations(data);
    } catch (error) {
      console.error('Failed to load invitations:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async (invitationId: number) => {
    setProcessingId(invitationId);
    try {
      await groupService.acceptInvitation(invitationId);
      setInvitations(invitations.filter(inv => inv.invitationId !== invitationId));
      alert('Invitation accepted! Refresh to see the group.');
      window.location.reload();
    } catch (error) {
      console.error('Failed to accept invitation:', error);
      alert('Failed to accept invitation');
    } finally {
      setProcessingId(null);
    }
  };

  const handleDecline = async (invitationId: number) => {
    setProcessingId(invitationId);
    try {
      await groupService.declineInvitation(invitationId);
      setInvitations(invitations.filter(inv => inv.invitationId !== invitationId));
    } catch (error) {
      console.error('Failed to decline invitation:', error);
      alert('Failed to decline invitation');
    } finally {
      setProcessingId(null);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center p-8">
        <div className="text-gray-500">Loading invitations...</div>
      </div>
    );
  }

  if (invitations.length === 0) {
    return (
      <div className="text-center p-8 text-gray-500">
        No pending invitations
      </div>
    );
  }

  return (
    <div className="space-y-3">
      {invitations.map((invitation) => (
        <div
          key={invitation.invitationId}
          className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow"
        >
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h3 className="font-semibold text-gray-900">{invitation.groupName}</h3>
              <p className="text-sm text-gray-600 mt-1">
                Invited by: {invitation.invitedBy}
              </p>
              <p className="text-xs text-gray-500 mt-1">
                {new Date(invitation.createdAt).toLocaleDateString()}
                {invitation.expiresAt && (
                  <span className="ml-2">
                    • Expires: {new Date(invitation.expiresAt).toLocaleDateString()}
                  </span>
                )}
              </p>
            </div>
            <div className="flex space-x-2 ml-4">
              <button
                onClick={() => handleAccept(invitation.invitationId)}
                disabled={processingId === invitation.invitationId}
                className="px-3 py-1.5 bg-green-500 hover:bg-green-600 text-white text-sm rounded transition-colors disabled:opacity-50"
              >
                Accept
              </button>
              <button
                onClick={() => handleDecline(invitation.invitationId)}
                disabled={processingId === invitation.invitationId}
                className="px-3 py-1.5 bg-red-500 hover:bg-red-600 text-white text-sm rounded transition-colors disabled:opacity-50"
              >
                Decline
              </button>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default InvitationList;
