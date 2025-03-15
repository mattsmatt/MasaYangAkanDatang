using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using JetBrains.Annotations;
using System;

public class InitializeSaveFile : MonoBehaviour
{
    void Awake()
    {
        // yang harus diinisialisasi waktu awal main, nama mc, npc
        // PlayerPrefs.DeleteKey("FirstRun");
        if (!PlayerPrefs.HasKey("FirstRun"))
        {
            // Logic to execute on the first run
            CreateNPCProgress();
            CreateSlideLevel();
            CreateSlideLevelList();
            CreateQuestData();
            CreatePlayerInventory();
            CreateCombatProgress();

            // Set the "FirstRun" key to prevent this from running again
            PlayerPrefs.SetInt("FirstRun", 1);
            PlayerPrefs.SetFloat("musicVolume", (float)0.4);
            PlayerPrefs.SetFloat("sfxVolume", (float)0.4);
            PlayerPrefs.SetFloat("sensitivity", (float)4);
            PlayerPrefs.SetInt("Arcade", 0);
            PlayerPrefs.SetInt("Component", 0);
            PlayerPrefs.SetInt("LockCombat", 0);
            PlayerPrefs.SetInt("UnlockCombat", 0);
            PlayerPrefs.Save(); // Save changes to PlayerPrefs
        }
    }

    public void CreateCombatProgress()
    {
        String x = "{\"lastUnlockedMinigame\":[1,1,1,1],\"lastUnlockedWorld\":1,\"lastUnlockedCombatLevel\":1}";
        SaveGame.Save("SaveData.dat", x);
    }

    public void CreateNPCProgress()
    {
        Dictionary<string, int> npcProgress = new Dictionary<string, int>{
            {"AboutCola",0},
            {"Alicia",0},
            {"AsepBurger",0},
            {"Beben",0},
            {"Bolang",0},
            {"Budiman",0},
            {"EndCutscene",0},
            {"Faqih",0},
            {"Ica",0},
            {"IniTesting",0},
            {"IntroAIRA",0},
            {"IntroGaguna",0},
            {"IntroMiduser",0},
            {"Miduser",0},
            {"Mira",0},
            {"SafiraBurger",0},
            {"SinarMonologue",0},
            {"Siti",0},

        };
        SaveGame.Save("NPCProgress", npcProgress);
    }

    public void CreatePlayerInventory()
    {
        Inventory inventory = new Inventory();
        // inventory.AddItem(0,1);
        // inventory.AddItem(1,1);
        // inventory.AddItem(2,1);
        SaveGame.Save("PlayerInventory", inventory);
    }

    public void CreateQuestData()
    {
        List<Quest> allquest = new List<Quest> {
            new Quest(
                1,
                "Berbicara Dengan Alicia",
                "Berbicaralah dengan Alicia untuk mencari informasi lebih lanjut mengenai tempat ini",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "berinteraksi dengan Alicia",1,0)
                },
                new List<int>()
                ),
            new Quest(
                2,
                "Ke Lab Alicia",
                "Pergi lah ke bagian kanan Laboratorium AIRA untuk bertemu dengan Alicia yang sudah menunggu di sana",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Menuju Lab Alicia",1,0)
                },
                new List<int>()
                ),
            new Quest(
                3,
                "Mencoba Simulasi Game",
                "Pergilah ke Miduser untuk mencoba simulasi combat game yang ada",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Berinteraksi dengan Miduser",1,0)
                },
                new List<int>()
                ),
            new Quest(
                4,
                "Mencari Alicia di Ruang Tamu",
                "Pergi lah ke tempat awal kamu bertemu dengan Alicia, dan tanyakan tentang list komponen mesin waktu",
                new Dictionary<int, int>{{4,1},{6,1}},
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Menuju Ruang Tamu",1,0)
                },
                new List<int>()
                ),
            new Quest(
                5,
                "Berinteraksi dengan panel AIRA",
                "Pergi lah ke bagian utara lab (lurus dari posisi awal bertemu Alicia) dan berinteraksi dengan panel yang ada disana",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "panel",1,0)
                },
                new List<int>()
                ),
            new Quest(
                6,
                "Mencari Toko Faqih",
                "Alicia menyarankan kamu untuk pergi ke Toko Faqih yang berada di Bengkel Gaguna (Gedung berwarna kuning)",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Mencari Faqih",1,0)
                },
                new List<int>()
                ),
            new Quest(
                7,
                "Mencari Toko Burger Budiman",
                "Faqih memberikan clue mengenai komponen berikutnya, Satelit yang dapat didapatkan di Budiman yang menjual burger di depan rumah biru di seberang pom bensin",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Toko Burger Budiman",1,0)
                },
                new List<int>()
                ),
            new Quest(
                8,
                "Mengumpulkan 3 Burger",
                "Cari dan kumpulkanlah 3 Burger dari gerobak burger di Kota Gaguna\n\nClue:\n- Di depan Rumah Sakit\n- Di dekat Faqih\n-Di Gedung Burger Shop(kuning)",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("collecting", "Burger Safira",1,7),
                    new QuestGoal("collecting", "Burger Ica",1,8),
                    new QuestGoal("collecting", "Burger Asep",1,9)
                },
                new List<int>{9}
                ),
            new Quest(
                9,
                "Kembali ke Toko Burger Budiman",
                "Kembali ke toko burger budiman untuk memberikan 3 burger tersebut. Toko Burger Budiman berada di depan rumah biru di seberang pom bensin",
                new Dictionary<int, int>{{11,1}},
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Toko Burger Budiman",1,0)
                },
                new List<int>()
                ),
            new Quest(
                10,
                "Menyelesaikan Game Arcade",
                "Kamu mendengar ada Game Arcade di taman yang memberikan Microchip antik sebagai hadiah pemenang game ini, carilah dan menangkan game ini",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Mesin Arcade",1,0)
                },
                new List<int>()
                ),
            new Quest(
                11,
                "Kembali ke AIRA",
                "kembali ke laboratorium AIRA dan berdiskusilah kepada Miduser untuk menganalisa Sirkuit tersebut",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Temui Miduser",1,0)
                },
                new List<int>()
                ),
            new Quest(
                12,
                "Kembali ke AIRA",
                "kembali ke laboratorium AIRA dan berdiskusilah kepada Miduser untuk menganalisa Satelit tersebut",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Temui Miduser",1,0)
                },
                new List<int>()
                ),
            new Quest(
                13,
                "Kembali ke AIRA",
                "kembali ke laboratorium AIRA dan berdiskusilah kepada Miduser untuk menganalisa Microchip tersebut",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Temui Miduser",1,0)
                },
                new List<int>()
                ),
            new Quest(
                14,
                "Menemui Pak Beben",
                "Pak Beben menyuruh kamu untuk menemuinya di ruangannya untuk membahas mesin waktu yang telah dia perbaiki",
                new Dictionary<int, int>(),
                new List<QuestGoal> {
                    new QuestGoal("interaction", "Temui Pak Beben",1,0)
                },
                new List<int>()
                )

        };
        SaveGame.Save("QuestData", allquest);
    }

    public void CreateSlideLevel()
    {
        List<PuzzleLevelData> slidelevel = new List<PuzzleLevelData>{
            new PuzzleLevelData(300,0,999),
            new PuzzleLevelData(301,0,999),
            new PuzzleLevelData(302,0,999),
            new PuzzleLevelData(303,0,999),
            new PuzzleLevelData(304,0,999)
        };
        SaveGame.Save("slidingLevel", slidelevel);
    }

    public void CreateSlideLevelList()
    {
        Inventory inv = new Inventory();
        inv.items.Add(new InventoryItem(300, 0));
        inv.items.Add(new InventoryItem(301, 0));
        inv.items.Add(new InventoryItem(302, 0));
        inv.items.Add(new InventoryItem(303, 0));
        inv.items.Add(new InventoryItem(304, 0));

        SaveGame.Save("slidingLevelList", inv);
    }
}
