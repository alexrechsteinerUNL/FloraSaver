﻿using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Utilities
{
    public static class AutoFillPlantGeneratorUtility
    {
        public static List<AutoFillPlant> AllAutoFillPlants;

        public static int ManuallySetCount = 1;
        public static void GenerateAutoFillPlants()
        {
            var autoFillPlants = new List<AutoFillPlant>();

            autoFillPlants.Add(new AutoFillPlant("African violet", 7, 3, 28, "https://www.thespruce.com/how-to-care-for-african-violets-1902776"));
            autoFillPlants.Add(new AutoFillPlant("Aloe vera", 14, 1, 56, "https://www.thespruce.com/grow-aloe-vera-indoors-1902775"));
            autoFillPlants.Add(new AutoFillPlant("Aluminum plant", 7, 3, 28, "https://www.thespruce.com/grow-aluminum-plant-indoors-1902774"));
            autoFillPlants.Add(new AutoFillPlant("Angels trumpet", 7, 3, 28, "https://www.thespruce.com/grow-angel-trumpet-indoors-1902773"));
            autoFillPlants.Add(new AutoFillPlant("Anthurium", 7, 3, 28, "https://www.thespruce.com/grow-anthurium-indoors-1902772"));
            autoFillPlants.Add(new AutoFillPlant("Areca palm", 7, 3, 28, "https://www.thespruce.com/grow-areca-palm-indoors-1902771"));
            autoFillPlants.Add(new AutoFillPlant("Arrowhead plant", 7, 3, 28, "https://www.thespruce.com/grow-arrowhead-plant-indoors-1902770"));
            autoFillPlants.Add(new AutoFillPlant("Asparagus fern", 7, 3, 28, "https://www.thespruce.com/grow-asparagus-fern-indoors-1902769"));
            autoFillPlants.Add(new AutoFillPlant("Baby's tears", 7, 3, 28, "https://www.thespruce.com/grow-babys-tears-indoors-1902768"));
            autoFillPlants.Add(new AutoFillPlant("Bamboo", 7, 3, 28, "https://www.thespruce.com/grow-bamboo-indoors-1902767"));
            autoFillPlants.Add(new AutoFillPlant("Begonia", 7, 3, 28, "https://www.thespruce.com/grow-begonias-indoors-1902765"));
            autoFillPlants.Add(new AutoFillPlant("Bird of paradise", 14, 1, 56, "https://www.thespruce.com/grow-bird-of-paradise-indoors-1902764"));
            autoFillPlants.Add(new AutoFillPlant("Boston fern", 7, 3, 28, "https://www.thespruce.com/grow-boston-fern-indoors-1902763"));
            autoFillPlants.Add(new AutoFillPlant("Parlor palm", 14, 1, 56, "https://www.thespruce.com/grow-parlor-palm-indoors-1902726"));
            autoFillPlants.Add(new AutoFillPlant("Bromeliad", 14, 1, 56, "https://www.thespruce.com/grow-bromeliads-indoors-1902697"));
            autoFillPlants.Add(new AutoFillPlant("Burros tail", 14, 1, 56, "https://www.thespruce.com/grow-burros-tail-indoors-1902696"));
            autoFillPlants.Add(new AutoFillPlant("Calathea", 7, 3, 28, "https://www.thespruce.com/grow-calathea-indoors-1902695"));
            autoFillPlants.Add(new AutoFillPlant("Cast iron plant", 14, 1, 56, "https://www.thespruce.com/grow-cast-iron-plant-indoors-1902694"));
            autoFillPlants.Add(new AutoFillPlant("Chinese evergreen", 14, 1, 56, "https://www.thespruce.com/grow-chinese-evergreen-indoors-1902693"));
            autoFillPlants.Add(new AutoFillPlant("Christmas cactus", 14, 1, 56, "https://www.thespruce.com/grow-christmas-cactus-indoors-1902692"));
            autoFillPlants.Add(new AutoFillPlant("Corn plant", 14, 1, 56, "https://www.thespruce.com/grow-corn-plant-indoors-1902691"));
            autoFillPlants.Add(new AutoFillPlant("Croton", 14, 1, 56, "https://www.thespruce.com/grow-croton-indoors-1902690"));
            autoFillPlants.Add(new AutoFillPlant("Dieffenbachia", 14, 1, 56, "https://www.thespruce.com/grow-dieffenbachia-indoors-1902689"));
            autoFillPlants.Add(new AutoFillPlant("Dumb cane", 14, 1, 56, "https://www.thespruce.com/grow-dumb-cane-indoors-1902688"));
            autoFillPlants.Add(new AutoFillPlant("Elephant ear", 7, 3, 28, "https://www.thespruce.com/grow-elephant-ear-plants-indoors-1902687"));
            autoFillPlants.Add(new AutoFillPlant("English ivy", 7, 3, 28, "https://www.thespruce.com/grow-english-ivy-indoors-1902686"));
            autoFillPlants.Add(new AutoFillPlant("Fiddle leaf fig", 14, 1, 56, "https://www.thespruce.com/grow-fiddle-leaf-fig-indoors-1902685"));
            autoFillPlants.Add(new AutoFillPlant("Flamingo flower", 7, 3, 28, "https://www.thespruce.com/grow-flamingo-flower-indoors-1902684"));
            autoFillPlants.Add(new AutoFillPlant("Fuchsia", 7, 3, 28, "https://www.thespruce.com/grow-fuchsia-indoors-1902683"));
            autoFillPlants.Add(new AutoFillPlant("Gardenia", 7, 3, 28, "https://www.thespruce.com/grow-gardenias-indoors-1902682"));
            autoFillPlants.Add(new AutoFillPlant("Geranium", 7, 3, 28, "https://www.thespruce.com/grow-geraniums-indoors-1902681"));
            autoFillPlants.Add(new AutoFillPlant("Grape ivy", 7, 3, 28, "https://www.thespruce.com/grow-grape-ivy-indoors-1902680"));
            autoFillPlants.Add(new AutoFillPlant("Haworthia", 14, 1, 56, "https://www.thespruce.com/grow-haworthia-indoors-1902679"));
            autoFillPlants.Add(new AutoFillPlant("Heartleaf philodendron", 7, 3, 28, "https://www.thespruce.com/grow-heartleaf-philodendron-indoors-1902678"));
            autoFillPlants.Add(new AutoFillPlant("Hoya", 14, 1, 56, "https://www.thespruce.com/grow-hoya-plants-indoors-1902679"));
            autoFillPlants.Add(new AutoFillPlant("Jade plant", 14, 1, 56, "https://www.thespruce.com/grow-jade-plants-indoors-1902677"));
            autoFillPlants.Add(new AutoFillPlant("Kalanchoe", 14, 1, 56, "https://www.thespruce.com/grow-kalanchoe-plants-indoors-1902676"));
            autoFillPlants.Add(new AutoFillPlant("Lace aloe", 14, 1, 56, "https://www.thespruce.com/grow-lace-aloe-indoors-1902675"));
            autoFillPlants.Add(new AutoFillPlant("Lady palm", 14, 1, 56, "https://www.thespruce.com/grow-lady-palm-indoors-1902674"));
            autoFillPlants.Add(new AutoFillPlant("Lipstick plant", 7, 3, 28, "https://www.thespruce.com/grow-lipstick-plant-indoors-1902673"));
            autoFillPlants.Add(new AutoFillPlant("Lucky bamboo", 7, 3, 28, "https://www.thespruce.com/grow-lucky-bamboo-indoors-1902672"));
            autoFillPlants.Add(new AutoFillPlant("Maidenhair fern", 7, 3, 28, "https://www.thespruce.com/grow-maidenhair-fern-indoors-1902671"));
            autoFillPlants.Add(new AutoFillPlant("Majesty palm", 7, 3, 28, "https://www.thespruce.com/grow-majesty-palm-indoors-1902670"));
            autoFillPlants.Add(new AutoFillPlant("Maranta", 7, 3, 28, "https://www.thespruce.com/grow-maranta-plants-indoors-1902669"));
            autoFillPlants.Add(new AutoFillPlant("Moth orchid", 7, 3, 28, "https://www.thespruce.com/grow-moth-orchids-indoors-1902668"));
            autoFillPlants.Add(new AutoFillPlant("Nerve plant", 7, 3, 28, "https://www.thespruce.com/grow-nerve-plant-indoors-1902667"));
            autoFillPlants.Add(new AutoFillPlant("Norfolk Island pine", 14, 1, 56, "https://www.thespruce.com/grow-norfolk-island-pine-indoors-1902666"));
            autoFillPlants.Add(new AutoFillPlant("Oxalis", 7, 3, 28, "https://www.thespruce.com/grow-oxalis-plants-indoors-1902665"));
            autoFillPlants.Add(new AutoFillPlant("Panda plant", 14, 1, 56, "https://www.thespruce.com/grow-panda-plant-indoors-1902664"));
            autoFillPlants.Add(new AutoFillPlant("Parlor palm", 14, 1, 56, "https://www.thespruce.com/grow-parlor-palm-indoors-1902663"));
            autoFillPlants.Add(new AutoFillPlant("Peace lily", 7, 3, 28, "https://www.thespruce.com/grow-peace-lily-indoors-1902662"));
            autoFillPlants.Add(new AutoFillPlant("Peperomia", 7, 3, 28, "https://www.thespruce.com/grow-peperomia-plants-indoors-1902661"));
            autoFillPlants.Add(new AutoFillPlant("Philodendron", 14, 1, 56, "https://www.thespruce.com/grow-philodendron-plants-indoors-1902660"));
            autoFillPlants.Add(new AutoFillPlant("Pilea", 7, 3, 28, "https://www.thespruce.com/grow-pilea-plants-indoors-1902659"));
            autoFillPlants.Add(new AutoFillPlant("Ponytail palm", 14, 1, 56, "https://www.thespruce.com/grow-ponytail-palm-indoors-1902658"));
            autoFillPlants.Add(new AutoFillPlant("Prayer plant", 7, 3, 28, "https://www.thespruce.com/grow-prayer-plants-indoors-1902657"));
            autoFillPlants.Add(new AutoFillPlant("Purple passion plant", 7, 3, 28, "https://www.thespruce.com/grow-purple-passion-plant-indoors-1902656"));
            autoFillPlants.Add(new AutoFillPlant("Rubber plant", 14, 1, 56, "https://www.thespruce.com/grow-rubber-plants-indoors-1902655"));
            autoFillPlants.Add(new AutoFillPlant("Sago palm", 14, 1, 56, "https://www.thespruce.com/grow-sago-palms-indoors-1902654"));
            autoFillPlants.Add(new AutoFillPlant("Sansevieria", 14, 1, 56, "https://www.thespruce.com/grow-sansevieria-snake-plant-indoors-1902653"));
            autoFillPlants.Add(new AutoFillPlant("Schefflera", 14, 1, 56, "https://www.thespruce.com/grow-schefflera-plants-indoors-1902652"));
            autoFillPlants.Add(new AutoFillPlant("Spider plant", 7, 3, 28, "https://www.thespruce.com/grow-spider-plants-indoors-1902651"));
            autoFillPlants.Add(new AutoFillPlant("Split-leaf philodendron", 14, 1, 56, "https://www.thespruce.com/grow-split-leaf-philodendron-indoors-1902650"));
            autoFillPlants.Add(new AutoFillPlant("String of pearls", 14, 1, 56, "https://www.thespruce.com/grow-string-of-pearls-indoors-1902649"));
            autoFillPlants.Add(new AutoFillPlant("Swedish ivy", 7, 3, 28, "https://www.thespruce.com/grow-swedish-ivy-indoors-1902648"));
            autoFillPlants.Add(new AutoFillPlant("Umbrella plant", 7, 3, 28, "https://www.thespruce.com/grow-umbrella-plants-indoors-1902647"));
            autoFillPlants.Add(new AutoFillPlant("Wandering Jew", 7, 3, 28, "https://www.thespruce.com/grow-wandering-jew-plants-indoors-1902646"));
            autoFillPlants.Add(new AutoFillPlant("Algerian ivy", 7, 3, 28, "https://www.thespruce.com/grow-algerian-ivy-indoors-1902640"));
            autoFillPlants.Add(new AutoFillPlant("Alocasia", 14, 1, 56, "https://www.thespruce.com/grow-alocasia-indoors-1902639"));
            autoFillPlants.Add(new AutoFillPlant("Arrowhead vine", 7, 3, 28, "https://www.thespruce.com/grow-arrowhead-vine-indoors-1902638"));
            autoFillPlants.Add(new AutoFillPlant("Bamboo palm", 14, 1, 56, "https://www.thespruce.com/grow-bamboo-palm-indoors-1902637"));
            autoFillPlants.Add(new AutoFillPlant("Begonia rex", 7, 3, 28, "https://www.thespruce.com/grow-begonia-rex-indoors-1902636"));
            autoFillPlants.Add(new AutoFillPlant("Bird's nest fern", 7, 3, 28, "https://www.thespruce.com/grow-birds-nest-fern-indoors-1902635"));
            autoFillPlants.Add(new AutoFillPlant("Pothos", 7, 3, 28, "https://www.thespruce.com/grow-pothos-devil-s-ivy-indoors-1902631"));
            autoFillPlants.Add(new AutoFillPlant("Weeping fig", 14, 1, 56, "https://www.thespruce.com/grow-weeping-fig-indoors-1902618"));
            autoFillPlants.Add(new AutoFillPlant("Yucca", 14, 1, 56, "https://www.thespruce.com/grow-yucca-indoors-1902617"));
            autoFillPlants.Add(new AutoFillPlant("Zebra plant", 7, 3, 28, "https://www.thespruce.com/grow-zebra-plants-indoors-1902616"));
            autoFillPlants.Add(new AutoFillPlant("ZZ plant", 14, 1, 56, "https://www.thespruce.com/grow-zz-plant-indoors-1902615"));
            autoFillPlants.Add(new AutoFillPlant("African mask plant", 14, 1, 56, "https://www.thespruce.com/grow-african-mask-plant-indoors-1902614"));
            Preferences.Default.Set("AutoFillPlantCount", autoFillPlants.Count);
            AllAutoFillPlants = autoFillPlants;
        }
    }
}