Table users {
  id uuid [pk]
  email varchar [unique]
  username varchar
  password_hash varbinary
  balance decimal(18,2)
}

Table farms {
  id uuid [pk]
  name varchar
  owner_id uuid [ref: > users.id]
}

Table animals {
  id uuid [pk]
  farm_id uuid [ref: > farms.id]
  species varchar
  name varchar
  birth_date datetime
  life_span_days int
  production_interval int
  next_production_at datetime
  purchase_price decimal(18,2)
  sell_price decimal(18,2)
  is_sold boolean
}

Table products {
  id uuid [pk]
  animal_id uuid [ref: > animals.id]
  product_type varchar
  sale_price decimal(18,2)
  produced_at datetime
}
