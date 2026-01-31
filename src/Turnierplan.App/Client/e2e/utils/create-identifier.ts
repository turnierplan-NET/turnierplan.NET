export const createIdentifier = () => {
  const adjectives = ['Vibrant', 'Mysterious', 'Glistening', 'Ancient', 'Whimsical', 'Fierce', 'Serene', 'Ethereal', 'Rustic', 'Dynamic'];
  const nouns = ['Galaxy', 'Whistle', 'Labyrinth', 'Oasis', 'Tapestry', 'Echo', 'Crescent', 'Voyage', 'Horizon', 'Pinnacle'];

  const index1 = Math.floor(Math.random() * adjectives.length);
  const index2 = Math.floor(Math.random() * nouns.length);
  const randomNumber = Math.floor(Math.random() * 90000 + 10000);

  return `${adjectives[index1]}_${nouns[index2]}_${randomNumber}`;
};
