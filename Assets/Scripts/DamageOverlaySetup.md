# Damage Overlay Setup Guide

## Option 1: Screen Space - Overlay (Recommended for VR)

### Step 1: Create Canvas
1. Right-click in Hierarchy → UI → Canvas
2. Name it "DamageOverlayCanvas"
3. Canvas Component Settings:
   - Render Mode: **Screen Space - Overlay**
   - Canvas Scaler → UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: 1920 x 1080 (or your target resolution)

### Step 2: Create Damage Overlay Image
1. Right-click on "DamageOverlayCanvas" → UI → Image
2. Name it "DamageOverlay"
3. Image Component Settings:
   - Color: Set Alpha to 0 (transparent)
   - Image Type: Simple
   - **Uncheck "Raycast Target"** (important - prevents blocking interactions)

### Step 3: Make Full Screen
1. Select "DamageOverlay" Image
2. In RectTransform:
   - Click anchor preset (top-left square)
   - Hold **Alt + Shift** and click bottom-right preset (stretch-stretch)
   - This anchors to all corners
   - Set Left, Right, Top, Bottom all to **0**

### Step 4: Set Canvas Sort Order
1. Select "DamageOverlayCanvas"
2. Set **Sort Order** to **100** (or any high number) to render on top

### Step 5: Assign to PlayerHealth Script
1. Select GameObject with PlayerHealth script
2. Drag "DamageOverlay" Image into **Damage Overlay** field in inspector

---

## Option 2: World Space Canvas (Alternative)

### Step 1: Create Canvas
1. Right-click in Hierarchy → UI → Canvas
2. Name it "DamageOverlayCanvas"
3. Canvas Component Settings:
   - Render Mode: **World Space**
   - Set Event Camera to your VR head camera

### Step 2: Position Canvas
1. Make it a child of your VR Camera/Head
2. Position it very close to camera (e.g., 0.5 units forward)
3. Scale it appropriately (usually very small, like 0.001 on all axes)

### Step 3-5: Same as Option 1

---

## Testing
- When a bug hits the player, you should see a red flash
- Adjust **Damage Color** and **Damage Flash Duration** in PlayerHealth inspector to customize

