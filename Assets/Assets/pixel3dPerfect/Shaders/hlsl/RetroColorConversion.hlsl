void RetroPaletteSelect_float(
    float4 color,
    float paletteID,
    float minLum,
    float maxLum,
    out float4 outColor)
{
    // 1) luminance Rec.709
    float lum = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
    float normLum = saturate((lum - minLum) / max(0.0001, maxLum - minLum));
    int pid = clamp((int) paletteID, 0, 5);

    float3 c;

    if (pid == 0)
    {
        // --- GAMEBOY (4 niveaux) ---
        const int L0 = 4;
        static const float3 pal0[L0] =
        {
            float3(15, 56, 15) / 255.0,
            float3(48, 98, 48) / 255.0,
            float3(139, 172, 15) / 255.0,
            float3(155, 188, 15) / 255.0
        };
        int idx = clamp((int) floor(normLum * L0), 0, L0 - 1);
        c = pal0[idx];
    }
    else if (pid == 1)
    {
       // --- NES (62 couleurs) ---
        const int L1 = 62;
        static const float3 pal1[L1] =
        {
            float3(84.0, 84.0, 84.0) / 255.0,
            float3(0.0, 30.0, 116.0) / 255.0,
            float3(8.0, 16.0, 144.0) / 255.0,
            float3(48.0, 0.0, 136.0) / 255.0,
            float3(68.0, 0.0, 100.0) / 255.0,
            float3(92.0, 0.0, 48.0) / 255.0,
            float3(84.0, 4.0, 0.0) / 255.0,
            float3(60.0, 24.0, 0.0) / 255.0,
            float3(32.0, 42.0, 0.0) / 255.0,
            float3(8.0, 58.0, 0.0) / 255.0,
            float3(0.0, 64.0, 0.0) / 255.0,
            float3(0.0, 60.0, 0.0) / 255.0,
            float3(0.0, 50.0, 60.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(152.0, 150.0, 152.0) / 255.0,
            float3(8.0, 76.0, 196.0) / 255.0,
            float3(48.0, 50.0, 236.0) / 255.0,
            float3(92.0, 30.0, 228.0) / 255.0,
            float3(136.0, 20.0, 176.0) / 255.0,
            float3(160.0, 20.0, 100.0) / 255.0,
            float3(152.0, 34.0, 32.0) / 255.0,
            float3(120.0, 60.0, 0.0) / 255.0,
            float3(84.0, 90.0, 0.0) / 255.0,
            float3(40.0, 114.0, 0.0) / 255.0,
            float3(8.0, 124.0, 0.0) / 255.0,
            float3(0.0, 118.0, 40.0) / 255.0,
            float3(0.0, 102.0, 120.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(236.0, 238.0, 236.0) / 255.0,
            float3(76.0, 154.0, 236.0) / 255.0,
            float3(120.0, 124.0, 236.0) / 255.0,
            float3(176.0, 98.0, 236.0) / 255.0,
            float3(228.0, 84.0, 236.0) / 255.0,
            float3(236.0, 88.0, 180.0) / 255.0,
            float3(236.0, 106.0, 100.0) / 255.0,
            float3(212.0, 136.0, 32.0) / 255.0,
            float3(160.0, 170.0, 0.0) / 255.0,
            float3(116.0, 196.0, 0.0) / 255.0,
            float3(76.0, 208.0, 32.0) / 255.0,
            float3(56.0, 204.0, 108.0) / 255.0,
            float3(56.0, 180.0, 204.0) / 255.0,
            float3(60.0, 60.0, 60.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(0.0, 0.0, 0.0) / 255.0,
            float3(236.0, 238.0, 236.0) / 255.0,
            float3(168.0, 204.0, 236.0) / 255.0,
            float3(188.0, 188.0, 236.0) / 255.0,
            float3(212.0, 178.0, 236.0) / 255.0,
            float3(236.0, 174.0, 236.0) / 255.0,
            float3(236.0, 174.0, 212.0) / 255.0,
            float3(236.0, 180.0, 176.0) / 255.0,
            float3(228.0, 196.0, 144.0) / 255.0,
            float3(204.0, 210.0, 120.0) / 255.0,
            float3(180.0, 222.0, 120.0) / 255.0,
            float3(168.0, 226.0, 144.0) / 255.0,
            float3(152.0, 226.0, 180.0) / 255.0,
            float3(160.0, 214.0, 228.0) / 255.0,
            float3(160.0, 162.0, 160.0) / 255.0
        };
        int idx = clamp((int) floor(normLum * L1), 0, L1 - 1);
        c = pal1[idx];
    }
    else if (pid == 2)
    {
        // --- Super NES (16 niveaux de gris “SNES Mode 7”) ---
        const int L2 = 16;
        static const float3 pal2[L2] =
        {
            // nuance de gris
            float3(0, 0, 0),
            float3(17, 17, 17),
            float3(34, 34, 34),
            float3(51, 51, 51),
            float3(68, 68, 68),
            float3(85, 85, 85),
            float3(102, 102, 102),
            float3(119, 119, 119),
            float3(136, 136, 136),
            float3(153, 153, 153),
            float3(170, 170, 170),
            float3(187, 187, 187),
            float3(204, 204, 204),
            float3(221, 221, 221),
            float3(238, 238, 238),
            float3(255, 255, 255)
        };
        int idx = clamp((int) floor(normLum * L2), 0, L2 - 1);
        c = pal2[idx] / 255.0;
    }
    else if (pid == 3)
    {
        // --- PC Engine (18 tons 9-bit RGB) ---
        const int L3 = 18;
        static const float3 pal3[L3] =
        {
            float3(0, 0, 0),
            float3(85, 0, 0),
            float3(170, 0, 0),
            float3(255, 0, 0),
            float3(0, 85, 0),
            float3(85, 85, 0),
            float3(170, 85, 0),
            float3(255, 85, 0),
            float3(0, 170, 0),
            float3(85, 170, 0),
            float3(170, 170, 0),
            float3(255, 170, 0),
            float3(0, 255, 0),
            float3(85, 255, 0),
            float3(170, 255, 0),
            float3(255, 255, 0),
            float3(255, 255, 255),
            float3(128, 128, 128)
        };
        int idx = clamp((int) floor(normLum * L3), 0, L3 - 1);
        c = pal3[idx] / 255.0;
    }
    else if (pid == 4)
    {
        // --- Sega Master System (16 tons bleutés) ---
        const int L4 = 16;
        static const float3 pal4[L4] =
        {
            float3(0, 0, 48),
            float3(0, 64, 128),
            float3(0, 128, 255),
            float3(0, 192, 255),
            float3(128, 0, 255),
            float3(192, 0, 255),
            float3(255, 0, 255),
            float3(255, 0, 192),
            float3(255, 0, 128),
            float3(255, 0, 0),
            float3(192, 64, 0),
            float3(128, 128, 0),
            float3(0, 255, 0),
            float3(0, 255, 128),
            float3(0, 255, 255),
            float3(128, 255, 255)
        };
        int idx = clamp((int) floor(normLum * L4), 0, L4 - 1);
        c = pal4[idx] / 255.0;
    }
    else /* pid == 5 */
    {
        // --- Atari 2600 (128 couleurs, on en prend 16 là) ---
        const int L5 = 16;
        static const float3 pal5[L5] =
        {
            float3(0, 0, 0),
            float3(74, 0, 0),
            float3(255, 0, 0),
            float3(255, 163, 0),
            float3(255, 255, 0),
            float3(0, 255, 0),
            float3(0, 0, 255),
            float3(128, 0, 255),
            float3(255, 0, 255),
            float3(255, 0, 128),
            float3(163, 73, 164),
            float3(128, 128, 128),
            float3(190, 190, 190),
            float3(255, 255, 255),
            float3(164, 164, 164),
            float3(100, 100, 100)
        };
        int idx = clamp((int) floor(normLum * L5), 0, L5 - 1);
        c = pal5[idx] / 255.0;
    }

    outColor = float4(c, color.a);
}
