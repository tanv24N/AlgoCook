
const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5089'

export async function getCuisines() {
  const res = await fetch(`${API_BASE}/api/cuisines`)
  return res.json()
}

export async function getByCuisine(cuisine) {
  const res = await fetch(`${API_BASE}/api/recipes/by-cuisine?cuisine=${encodeURIComponent(cuisine)}`)
  return res.json()
}

export async function getByIngredients(ingredientsCsv) {
  const res = await fetch(`${API_BASE}/api/recipes/by-ingredients?ingredients=${encodeURIComponent(ingredientsCsv)}`)
  return res.json()
}

export async function getRecipe(id) {
  const res = await fetch(`${API_BASE}/api/recipes/${id}`)
  return res.json()
}

export async function listFavorites() {
  const res = await fetch(`${API_BASE}/api/favorites`)
  return res.json()
}

export async function addFavorite(fav) {
  const res = await fetch(`${API_BASE}/api/favorites`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(fav)
  })
  return res.json()
}

export async function deleteFavorite(id) {
  await fetch(`${API_BASE}/api/favorites/${id}`, { method: 'DELETE' })
}
