using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls;
using MusicDownloader.Mvvm.Infrastructure;
using System.Collections.ObjectModel;
using MusicDownloader.Models;
using MusicDownloader.Services;
using System;

namespace MusicDownloader.Mvvm.ViewModels
{
    public class Person
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public ObservableCollection<Person> Children { get; } = new();
    }

    public sealed class ProfileDisplayViewModel : ViewModelBase
    {
        /// <summary>
        /// Whether the folder is initialized.
        /// </summary>
        public bool IsMusicFolderInitialized => _profileStateProvider.IsProfileInitialized();

        private readonly IProfileStateProvider _profileStateProvider;
        private readonly ProfileState Profile;
        private ObservableCollection<Person> _people;

        public HierarchicalTreeDataGridSource<Person> PersonSource { get; }

        public ProfileDisplayViewModel(IProfileStateProvider profileStateProvider)
        {
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));

            _people = new ObservableCollection<Person>()
            {
                new Person
                {
                    FirstName = "Eleanor",
                    LastName = "Pope",
                    Age = 32,
                    Children =
                    {
                        new Person
                        {
                            FirstName = "Marcel",
                            LastName = "Gutierrez",
                            Age = 4
                        },
                    }
                },
                new Person
                {
                    FirstName = "Jeremy",
                    LastName = "Navarro",
                    Age = 74,
                    Children =
                    {
                        new Person
                        {
                            FirstName = "Jane",
                            LastName = "Navarro",
                            Age = 42 ,
                            Children =
                            {
                                new Person
                                {
                                    FirstName = "Lailah ",
                                    LastName = "Velazquez",
                                    Age = 16
                                }
                            }
                        },
                    }
                },
                new Person
                {
                    FirstName = "Jazmine",
                    LastName = "Schroeder",
                    Age = 52
                },
            };

            PersonSource = new HierarchicalTreeDataGridSource<Person>(_people)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<Person>(
                        new TextColumn<Person, string>
                            ("First Name", x => x.FirstName, GridLength.Parse("200")),x => x.Children),
                    new TextColumn<Person, string>
                            ("Last Name", x => x.LastName, GridLength.Parse("300")),
                    new TextColumn<Person, int>("Age", x => x.Age, GridLength.Parse("400")),
                },
            };
        }
    }
}