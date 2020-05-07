﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackOfficeBase.Application.Crud;
using BackOfficeBase.Application.OrganizationUnits.Dto;
using BackOfficeBase.DataAccess;
using BackOfficeBase.Domain.Entities.OrganizationUnits;

namespace BackOfficeBase.Application.OrganizationUnits
{
    // TODO: Write unit tests
    public class OrganizationUnitAppService : CrudAppService<OrganizationUnit, OrganizationUnitOutput, OrganizationUnitListOutput, CreateOrganizationUnitInput, UpdateOrganizationUnitInput>, IOrganizationUnitAppService
    {
        private readonly BackOfficeBaseDbContext _dbContext;

        public OrganizationUnitAppService(BackOfficeBaseDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public async Task<OrganizationUnitOutput> AddUsersToOrganizationUnitAsync(AddOrRemoveUsersToOrganizationUnitInput input)
        {
            foreach (var selectedUserId in input.SelectedUserIds)
            {
                await _dbContext.OrganizationUnitUsers.AddAsync(new OrganizationUnitUser
                {
                    UserId = selectedUserId,
                    OrganizationUnitId = input.OrganizationUnitId
                });
            }

            return await base.GetAsync(input.OrganizationUnitId);
        }

        public async Task<OrganizationUnitOutput> RemoveUsersFromOrganizationUnit(AddOrRemoveUsersToOrganizationUnitInput input)
        {
            _dbContext.OrganizationUnitUsers.RemoveRange(_dbContext.OrganizationUnitUsers.Where(x =>
                    input.SelectedUserIds.Contains(x.UserId) && x.OrganizationUnitId == input.OrganizationUnitId));

                return await base.GetAsync(input.OrganizationUnitId);
        }

        public async Task<OrganizationUnitOutput> AddRolesToOrganizationUnitAsync(AddOrRemoveRolesToOrganizationUnitInput input)
        {
            foreach (var selectedRoleId in input.SelectedRoleIds)
            {
                await _dbContext.OrganizationUnitRoles.AddAsync(new OrganizationUnitRole
                {
                    RoleId = selectedRoleId,
                    OrganizationUnitId = input.OrganizationUnitId
                });
            }

            return await base.GetAsync(input.OrganizationUnitId);
        }

        public async Task<OrganizationUnitOutput> RemoveRolesFromOrganizationUnit(AddOrRemoveRolesToOrganizationUnitInput input)
        {
            _dbContext.OrganizationUnitRoles.RemoveRange(_dbContext.OrganizationUnitRoles.Where(x =>
                input.SelectedRoleIds.Contains(x.RoleId) && x.OrganizationUnitId == input.OrganizationUnitId));

            return await base.GetAsync(input.OrganizationUnitId);
        }
    }
}