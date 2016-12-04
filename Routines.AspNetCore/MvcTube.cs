using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Vse.Routines.AspNetCore
{
    public class MvcTube
    {
        readonly Controller controller;
        public MvcTube(Controller controller)
        {
            this.controller = controller;
        }

        public IActionResult Handle<TEntity>(Func<bool> isValidInput, Func<TEntity> getEntity, Action<TEntity> prepareRendering = null)
        {
            if (!isValidInput())
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            var entity = getEntity();
            if (entity == null)
                return controller.NotFound();
            prepareRendering?.Invoke(entity);
            return controller.View(entity);
        }
    }
}
