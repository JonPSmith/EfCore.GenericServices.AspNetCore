// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace ExampleDatabase
{
    public class TodoItem
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// This represents the difficulty of setting it up: 1 = easy, 5 = hard
        /// </summary>
        [Range(1, 5)]
        public int Difficulty { get; set; }

        public TodoItem() 
        {
        }

        public TodoItem(string name, int difficulty)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Difficulty = difficulty;
        }
    }
}