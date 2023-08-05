using AIChefRemsey.Shared;
using System;

namespace AIChefRemsey.Server.Data
{
    public class SampleData
    {
        public static List<Idee> ReceptIdeeën = new()
        {
            new Idee
            {
                index = 0,
                title = "Chocolate Chip Cookies",
                description = "Delicious chocolate chip cookies made with browned butter"
            },
            new Idee
            {
                index = 1,
                 title = "Peanut Butter Cookies",
                 description= "Cookies made with peanut butter and butterscotch chips. Yum!"
            },
            new Idee {
                index = 2,
               title = "Snickerdoodles",
                 description = "Classic snickerdoodle cookies. The secret ingredient is cream of tartar!"
            },
            new Idee {
                index = 2,
               title= "Sugar Cookies",
                 description= "A sugar cookie is a cookie with the main ingredients being sugar, flour, butter, eggs, vanilla, and either baking powder or baking soda."
            },
            new Idee {
                index = 2,
                 title = "Ginger Snaps",
                 description = "Ginger snaps are a classic favorite. With just a few ingredients and even fewer steps this recipe for fabulous, spicy cookies is truly a snap to make."
            },
        };

        public static Recept recept = new()
        {
            title="Ginger Snaps",
            summary = "Ginger snaps are a classic favorite. With just a few ingredients and",
            ingredients = new[]
                {
                "1 cup packed brown sugar",
                "3/4 cup vegetable oil",
                "1/4 cup molasses",
                "1 large egg",
                "2 cups all-purpose flour",
                "2 teaspoons baking soda",
                "1 teaspoon ground ginger",
                "1 teaspoon ground cinnamon",
                "3/4 teaspoon ground cloves",
                "1/4 teaspoon salt",
            },
            instructions = new[]
            {

                "Pr Show potential fixes (Alt+Enter or Ctrl+.) es F (190 degrees C).",
                "Mix together brown sugar, oil, molasses, and egg in a large bowl.",
                "Combine flour, baking soda, ginger, cinnamon, cloves, and salt; stir into",
                "Roll dough into 1 1/4-inch balls. Roll each ball in white sugar before ",
                "Bake for 10 to 12 minutes in the preheated oven, or until center is firm."

            }
        };

        public static ReceptAfbeelding Receptafbeelding = new()
        {

            data = new ImageData[]
            {
                new ImageData()
                {
                    url = "https://imagesvc.meredithcorp.io/v3/jumpstartpure/image/?url=https%3A%2F%2Fstatic.onecms.io%2Fwp-content%2Fuploads%2Fsites%2F43%2F2023%2F01%2F23%2F222584-bacon-cheese-frittata-4x3-0775.jpg&w=640&h=360&q=90&c=cc"
                }
            }
        };
    }
}
