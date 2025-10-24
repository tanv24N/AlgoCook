
import React, { useEffect, useState } from 'react'
import { getCuisines, getByCuisine, getByIngredients, getRecipe, addFavorite, listFavorites, deleteFavorite } from './api'

function Tabs({ value, onChange }) {
  const tabs = ['Cuisine','Ingredients','Favorites']
  return (
    <div style={{ display:'flex', gap:8, margin:'12px 0' }}>
      {tabs.map(tab => (
        <button
          key={tab}
          onClick={()=>onChange(tab)}
          style={{ padding: '8px 12px', borderRadius: 8, border: value===tab?'2px solid black':'1px solid #aaa', background: value===tab?'#eee':'white', cursor:'pointer' }}
        >
          {tab}
        </button>
      ))}
    </div>
  )
}

function Card({ r, onOpen, onFav }) {
  return (
    <div style={{ border:'1px solid #ddd', padding:10, borderRadius:12, display:'flex', flexDirection:'column', gap:8 }}>
      <img src={r.Thumbnail} alt={r.Title} style={{ width:'100%', borderRadius:8, objectFit:'cover', aspectRatio:'4/3' }} />
      <b>{r.Title}</b>
      <div style={{ display:'flex', gap:8 }}>
        <button onClick={onOpen}>View</button>
        <button onClick={onFav}>Favorite</button>
      </div>
    </div>
  )
}

function Details({ id, onClose }) {
  const [data,setData] = useState(null)
  useEffect(()=>{ getRecipe(id).then(setData) },[id])
  if(!data) return <div>Loading…</div>
  return (
    <div style={{ padding:12 }}>
      <h3>{data.Title}</h3>
      <img src={data.Thumbnail} alt={data.Title} style={{ width:'100%', borderRadius:8, objectFit:'cover', aspectRatio:'4/3' }} />
      <p><b>Category:</b> {data.Category} | <b>Area:</b> {data.Area}</p>
      <h4>Ingredients</h4>
      <ul>
        {data.Ingredients.map((ing,i)=> <li key={i}>{ing} {data.Measures[i] ? `- ${data.Measures[i]}` : ''}</li>)}
      </ul>
      <h4>Instructions</h4>
      <p style={{ whiteSpace:'pre-wrap' }}>{data.Instructions}</p>
      {data.YoutubeUrl && <p><a href={data.YoutubeUrl} target="_blank" rel="noreferrer">YouTube</a></p>}
      {data.SourceUrl && <p><a href={data.SourceUrl} target="_blank" rel="noreferrer">Source</a></p>}
      <button onClick={onClose}>Close</button>
    </div>
  )
}

export default function App(){
  const [tab,setTab] = useState('Cuisine')

  // Cuisine tab
  const [cuisines,setCuisines] = useState([])
  const [selected,setSelected] = useState('Italian')
  const [results,setResults] = useState([])

  // Ingredients tab
  const [ingText,setIngText] = useState('chicken, garlic')
  const [ingResults,setIngResults] = useState([])

  // Modal
  const [openId,setOpenId] = useState(null)

  // Favorites
  const [favs,setFavs] = useState([])
  const refreshFavs = () => listFavorites().then(setFavs)

  useEffect(()=>{ getCuisines().then(setCuisines) },[])
  useEffect(()=>{ if(tab==='Favorites') refreshFavs() },[tab])

  const searchCuisine = async () => {
    const list = await getByCuisine(selected)
    setResults(list)
  }

  const searchIngredients = async () => {
    const list = await getByIngredients(ingText)
    setIngResults(list)
  }

  const grid = (items) => (
    <div style={{ display:'grid', gridTemplateColumns:'repeat(auto-fill, minmax(220px,1fr))', gap:12 }}>
      {items.map(x => (
        <Card
          key={x.Id}
          r={{Title:x.Title, Thumbnail:x.Thumbnail}}
          onOpen={()=>setOpenId(x.Id)}
          onFav={()=>addFavorite({ recipeId:x.Id, title:x.Title, thumbUrl:x.Thumbnail }).then(()=>alert('Saved!'))}
        />
      ))}
    </div>
  )

  return (
    <div style={{ maxWidth:1000, margin:'0 auto', padding:16, fontFamily:'system-ui, Arial' }}>
      <h2>AI Recipe Finder</h2>
      <p style={{color:'#555'}}>Search by cuisine or ingredients. Save favorites. 100% free stack.</p>

      <Tabs value={tab} onChange={setTab} />

      {tab==='Cuisine' && (
        <div>
          <div style={{ display:'flex', gap:8, alignItems:'center' }}>
            <label>Choose cuisine:</label>
            <select value={selected} onChange={e=>setSelected(e.target.value)}>
              {cuisines.map(c => <option key={c} value={c}>{c}</option>)}
            </select>
            <button onClick={searchCuisine}>Search</button>
          </div>
          <div style={{ marginTop:12 }}>{grid(results)}</div>
        </div>
      )}

      {tab==='Ingredients' && (
        <div>
          <div style={{ display:'flex', gap:8, alignItems:'center' }}>
            <label>Ingredients:</label>
            <input value={ingText} onChange={e=>setIngText(e.target.value)} placeholder="e.g., chicken, tomato, garlic" style={{ flex:1 }} />
            <button onClick={searchIngredients}>Search</button>
          </div>
          <small style={{color:'#666'}}>Tip: enter comma-separated ingredients. Results show recipes that contain all of them.</small>
          <div style={{ marginTop:12 }}>{grid(ingResults)}</div>
        </div>
      )}

      {tab==='Favorites' && (
        <div>
          <button onClick={refreshFavs}>Refresh</button>
          <div style={{ display:'grid', gridTemplateColumns:'repeat(auto-fill, minmax(220px,1fr))', gap:12, marginTop:12 }}>
            {favs.map(f => (
              <div key={f.Id} style={{ border:'1px solid #ddd', padding:10, borderRadius:12 }}>
                <img src={f.ThumbUrl} alt={f.Title} style={{ width:'100%', borderRadius:8, objectFit:'cover', aspectRatio:'4/3' }} />
                <b>{f.Title}</b>
                <div style={{ display:'flex', gap:8, marginTop:8 }}>
                  <button onClick={()=>deleteFavorite(f.Id).then(refreshFavs)}>Remove</button>
                  <button onClick={()=>setOpenId(f.RecipeId)}>View</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {openId && (
        <div style={{ position:'fixed', inset:0, background:'rgba(0,0,0,0.5)', display:'grid', placeItems:'center' }} onClick={()=>setOpenId(null)}>
          <div style={{ background:'white', width:'min(800px, 95vw)', maxHeight:'90vh', overflow:'auto', borderRadius:12 }} onClick={e=>e.stopPropagation()}>
            <Details id={openId} onClose={()=>setOpenId(null)} />
          </div>
        </div>
      )}
    </div>
  )
}
