// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using ExampleDatabase;
using GenericServices;

namespace ExampleWebApi.Dtos
{
    public class CreateTodoHybridDto : ILinkToEntity<TodoItemHybrid>
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        /// <summary>
        /// This represents the difficulty of setting it up: 1 = easy, 5 = hard
        /// </summary>
        [Range(1, 5)]
        public int Difficulty { get; set; }
    }
}