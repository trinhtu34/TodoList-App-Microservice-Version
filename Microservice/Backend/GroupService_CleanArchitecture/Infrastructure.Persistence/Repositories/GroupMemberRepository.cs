using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GroupMemberRepository : IGroupMemberRepository
{
    private readonly GroupServiceDbContext _context;

    public GroupMemberRepository(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<GroupMember?> GetByIdAsync(int groupId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
    }

    public async Task<List<GroupMember>> GetGroupMembersAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .Where(m => m.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<GroupMember> AddAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);
        return member;
    }

    public async Task UpdateAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Update(member);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GroupMember member, CancellationToken cancellationToken = default)
    {
        _context.GroupMembers.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
