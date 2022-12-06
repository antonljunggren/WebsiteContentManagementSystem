using Core.Models;
using Core.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public sealed class PhotographService
    {
        private readonly CmsContext _cmsContext;

        public PhotographService(IDbContextFactory<CmsContext> contextFactory)
        {
            _cmsContext = contextFactory.CreateDbContext();
        }

        public async Task<Photograph> Get(string id)
        {
            var photograph = await _cmsContext.Photographs.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
            if(photograph == null)
            {
                throw new NullReferenceException($"Photograph with id: {id} does not exist!");
            }

            return photograph;
        }

        public async Task<List<Photograph>> GetAll()
        {
            var photographs = await _cmsContext.Photographs.AsNoTracking().ToListAsync();

            return photographs;
        }

        public async Task DeletePhotograph(string photoId)
        {
            var photo = await _cmsContext.Photographs.SingleOrDefaultAsync(p => p.Id == photoId);

            if (photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {photoId} does not exist!");
            }

            _cmsContext.Photographs.Remove(photo);
            await _cmsContext.SaveChangesAsync();
        }

        public async Task<Photograph> AddPhotograph(Photograph photograph)
        {
            photograph.Id = ShortStringIdService.Generate(11);

            var res = await _cmsContext.Photographs.AddAsync(photograph);
            await _cmsContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<Photograph> UpdatePhotograph(Photograph newPhoto)
        {
            var photo = await _cmsContext.Photographs.SingleOrDefaultAsync(p => p.Id == newPhoto.Id);

            if (photo == null)
            {
                throw new NullReferenceException($"Photograph with id: {newPhoto.Id} does not exist!");
            }

            photo.Title= newPhoto.Title;
            photo.Description= newPhoto.Description;
            photo.Location = newPhoto.Location;
            photo.FilmUsed = newPhoto.FilmUsed;
            photo.CameraUsed = newPhoto.CameraUsed;
            photo.DateTaken = newPhoto.DateTaken;
            photo.HdImageData = newPhoto.HdImageData;
            photo.SdImageData = newPhoto.SdImageData;
            photo.IsPublished = newPhoto.IsPublished;

            var res = _cmsContext.Photographs.Update(photo);
            await _cmsContext.SaveChangesAsync();

            return res.Entity;
        }
    }
}
