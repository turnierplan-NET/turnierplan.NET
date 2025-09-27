// Generated using AI
export const teamNames = [
  'River City Rangers',
  'Mountain View United',
  'Seaside Strikers',
  'Valley Forge FC',
  'Urban Titans',
  'Desert Eagles',
  'Lakeside Legends',
  'Skyline Spartans',
  'Forest Hill FC',
  'Coastal Warriors',
  'Highland Harriers',
  'Central City Cobras',
  'Golden Coast FC',
  'Northern Stars United',
  'Southern Phoenix',
  'Eastwood Explorers',
  'Westside Wolves',
  'Crystal Bay Crusaders',
  'Maplewood Mavericks',
  'Pine Valley Pioneers'
];

export const createIdentifier = () => {
  const adjectives = ['Vibrant', 'Mysterious', 'Glistening', 'Ancient', 'Whimsical', 'Fierce', 'Serene', 'Ethereal', 'Rustic', 'Dynamic'];
  const nouns = ['Galaxy', 'Whistle', 'Labyrinth', 'Oasis', 'Tapestry', 'Echo', 'Crescent', 'Voyage', 'Horizon', 'Pinnacle'];

  const index1 = Math.floor(Math.random() * adjectives.length);
  const index2 = Math.floor(Math.random() * nouns.length);
  const randomNumber = Math.floor(Math.random() * 9000 + 1000);

  return `${adjectives[index1]}_${nouns[index2]}_${randomNumber}`;
};
