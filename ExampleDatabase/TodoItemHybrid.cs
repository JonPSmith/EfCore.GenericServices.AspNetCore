// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using GenericServices;

namespace ExampleDatabase
{
    public class TodoItemHybrid
    {
        public int Id { get; private set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; private set; }

        /// <summary>
        /// This represents the difficulty of setting it up: 1 = easy, 5 = hard
        /// </summary>
        [Range(1, 5)]
        public int Difficulty { get; set; }

        //for EF Core
        private TodoItemHybrid() {}

        public TodoItemHybrid(string name, int difficulty)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Difficulty = difficulty;
        }

        public IStatusGeneric ChangeName(string name)
        {
            var status = new StatusGenericHandler();
            if (name.EndsWith("!"))
                status.AddError("Business logic says the name cannot end with !", nameof(Name));

            Name = name;
            return status;
        }
    }
}