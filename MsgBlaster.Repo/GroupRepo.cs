using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class GroupRepo : GenericRepository<Group>
    {
        internal GroupRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        //public override Group GetById(int id)
        //{
        //    return GetById(id, true);
        //}
        //public override Group GetById(int id, bool loadAll)
        //{
        //    if (loadAll)
        //    {
        //        return base.GetById(id, "Contacts");
        //    }
        //    return base.GetById(id);
        //}

        protected override void BeforeInsert(Group entity)
        {
           
        }
        protected override void BeforeUpdate(Group entity)
        {
           
        }
        protected override void BeforeDelete(Group entity)
        {
            var id = entity.Id;
            if (_context.Campaigns.Any(p => p.GroupId == id) || _context.EcouponCampaigns.Any(p => p.GroupId == id))
                throw new Exception("Cannot delete group when child entities exist");
        }

        //public void AddContact(int groupId, int contactId)
        //{
        //    var group = GetById(groupId, true);
        //    var contact = _uow.ContactRepo.GetById(contactId);
        //    if (group == null)
        //        throw new msgBlasterValidationException("No such Group");
        //    if (contact == null)
        //        throw new msgBlasterValidationException("No such contact");

        //    group.Contacts.Add(contact);
        //    _context.Groups.AddOrUpdate(group);
        //}

        //public void RemoveContact(int groupId, int contactId)
        //{
        //    var group = GetById(groupId, true);
        //    if (group == null)
        //        throw new msgBlasterValidationException("No such group");

        //    var contact = group.Contacts.FirstOrDefault(p => p.Id == contactId);
        //    if (contact == null)
        //        throw new msgBlasterValidationException("No such contact");

        //    group.Contacts.Remove(contact);
        //    //contact.Groups.Remove(group);

        //   _context.Groups.AddOrUpdate(group);
        //   // _context.Contacts.AddOrUpdate(contact);


        //}

    }
}
