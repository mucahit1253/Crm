﻿using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CampaignManager : ICampaignService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public CampaignManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
        }



        public async Task<CampaignDto> CreateOneCampaignAsync(CampaignDtoForInsertion campaignDto)
        {
            var entity =_mapper.Map<Campaign>(campaignDto);
            _manager.Campaign.CreateOneCampaign(entity);
            await _manager.SaveAsync();
            return _mapper.Map<CampaignDto>(entity);
        }

        public async Task DeleteOneCampaignAsync(int id, bool trackChanhes)
        {

            var entity = await GetOneCampaignByIdAndCheckExists(id, trackChanhes);

            _manager.Campaign.DeleteOneCampaign(entity);
            await _manager.SaveAsync();
        }

        public async Task<(IEnumerable<CampaignDto> campaigns,MetaData metaData)> GetAllCampaignAsync(CampaignParameters campaignParameters,
            bool trackChanhes)
        {
            if (!campaignParameters.ValidDateRange)
                throw new DateTimeOutofRangeBadRequest();
            var campaignsWithMetaData=await _manager
                .Campaign
                .GetAllCampaingAsync(campaignParameters,trackChanhes);
            var campaignsDto=_mapper.Map<IEnumerable<CampaignDto>>(campaignsWithMetaData);
            return (campaignsDto, campaignsWithMetaData.MetaData);
        }

        public async Task<CampaignDto>GetOneCampaignByIdAsync(int id, bool trackChanhes)
        {
            var campaign = await GetOneCampaignByIdAndCheckExists(id, trackChanhes);
            
            return _mapper.Map<CampaignDto>(campaign);
        }

        public async Task<(CampaignDtoForUpdate campaignDtoForUpdate, Campaign campaign)> 
            GetOneCampaignForPatchAsync(int id, bool trackChanges)
        {
            var campaign = await GetOneCampaignByIdAndCheckExists(id,trackChanges);
            
            var campaignDtoForUpdate=_mapper.Map<CampaignDtoForUpdate>(campaign);
            return (campaignDtoForUpdate,campaign);

        }

        public async Task SaveChangesForPatchAsync(CampaignDtoForUpdate campaignDtoForUpdate, Campaign campaign)
        {
            _mapper.Map(campaignDtoForUpdate, campaign);
            await _manager.SaveAsync();
        }

        public async Task UpdateOneCampaignAsync(int id, 
            CampaignDtoForUpdate campaignDto,
            bool trackChanges)
        {
            var entity = await GetOneCampaignByIdAndCheckExists(id, trackChanges);

            

            //mapping
            entity = _mapper.Map<Campaign>(campaignDto);
            _manager.Campaign.Update(entity);
            await _manager.SaveAsync();
        }
        private async Task<Campaign> GetOneCampaignByIdAndCheckExists(int id, bool trackChanges)
        {
            // check entity 
            var entity = await _manager.Campaign.GetOneCampaingByIdAsync(id, trackChanges);

            if (entity is null)
                throw new CampaingNotFoundException(id);

            return entity;
        }
    }
}

