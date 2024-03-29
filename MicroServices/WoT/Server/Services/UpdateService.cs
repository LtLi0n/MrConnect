﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using System.Threading.Tasks;

namespace WoT.Server
{
    public class UpdateService
    {
        private readonly IServiceProvider _services;
        public CharacterWorkService CharacterWorkService { get; }

        private readonly Timer _timer;
        private bool _updating;

        public UpdateService(IServiceProvider services, CharacterWorkService workService)
        {
            _services = services;
            CharacterWorkService = workService;

            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_updating) return;
            else _updating = true;

            List<Task> updates = new List<Task>
            {
                CharacterWorkService.UpdateAsync(_services)
            };

            foreach(Task update in updates)
            {
                await update;
            }

            _updating = false;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
