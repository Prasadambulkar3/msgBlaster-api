using System.Data.Entity.Migrations;
using System.Linq;
using MsgBlaster.Domain;
using System;

namespace MsgBlaster.Repo
{
    public class ContactRepo : GenericRepository<Contact>
    {

        internal ContactRepo(UnitOfWork uow)
            : base(uow)
        {
        }

        protected override void BeforeInsert(Contact entity)
        {

        }
        protected override void BeforeUpdate(Contact entity)
        {
            
        }
        protected override void BeforeDelete(Contact entity)
        {
            var id = entity.Id;

            if(_context.GroupContacts.Any(p=>p.ContactId == id))
            throw new Exception("Cannot delete group when child entities exist");


            //if (_context.Groups.Any(p => p.Contacts.Any(d => d.Id == id)))
            //    throw new Exception("Cannot delete group when child entities exist");
        }


        //public void AddGroup(int contactId, int groupId)
        //{
        //    var contact = GetById(contactId, true);
        //    var group = _uow.GroupRepo.GetById(groupId);
        //    if (group == null)
        //        throw new msgBlasterValidationException("No such group");
        //    if (contact == null)
        //        throw new msgBlasterValidationException("No such contact");

        //    contact.Groups.Add(group);
        //    _context.Contacts.AddOrUpdate(contact);
        //}
        //public void RemoveGroup(int grouptId, int contactId)
        //{
        //    var contact = GetById(contactId, true);
        //    if (contact == null)
        //        throw new msgBlasterValidationException("No such contact");

        //    var group = contact.Groups.FirstOrDefault(d => d.Id == grouptId);
        //    if (group == null)
        //        throw new msgBlasterValidationException("No such group");

        //    contact.Groups.Remove(group);
        //    _context.Contacts.AddOrUpdate(contact);
        //}


    }
}
